using System;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using GroupNStegafy.IO;
using Windows.UI;
using Windows.UI.ViewManagement;

namespace GroupNStegafy.View
{
    /// <summary>
    ///     The embedding page used for embedding images
    /// </summary>
    public sealed partial class EmbedMessagePage : Page
    {
        #region Data members

        private readonly double applicationHeight = (double)Application.Current.Resources["AppHeight"];
        private readonly double applicationWidth = (double)Application.Current.Resources["AppWidth"];

        private double dpiX;
        private double dpiY;
        private WriteableBitmap embeddedImage;
        private StorageFile messageImageFile;
        private StorageFile sourceImageFile;
        private readonly FileWriter fileWriter;
        private readonly FileReader fileReader;
        private bool messageImageTooLarge;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="EmbedMessagePage" /> class.
        /// </summary>
        public EmbedMessagePage()
        {
            this.InitializeComponent();

            ApplicationView.PreferredLaunchViewSize = new Size
            { Width = this.applicationWidth, Height = this.applicationHeight };
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            ApplicationView.GetForCurrentView()
                           .SetPreferredMinSize(new Size(this.applicationWidth, this.applicationHeight));

            this.sourceImageFile = null;
            this.messageImageFile = null;
            this.embeddedImage = null;
            this.messageImageTooLarge = false;
            this.dpiX = 0;
            this.dpiY = 0;

            this.fileWriter = new FileWriter();
            this.fileReader = new FileReader();
        }

        #endregion

        #region Methods

        private async void loadSourceButton_Click(object sender, RoutedEventArgs e)
        {
            this.sourceImageFile = await this.fileReader.SelectSourceImageFile();
            if (this.sourceImageFile == null)
            {
                return;
            }

            var sourceImage = await this.convertToBitmap(this.sourceImageFile);
            this.sourceImageDisplay.Source = sourceImage;

            if (this.sourceImageFile != null && this.messageImageFile != null)
            {
                this.enableSettingsOptions();
            }
        }

        private async void loadMessageButton_Click(object sender, RoutedEventArgs e)
        {
            var messageImageFile = await this.fileReader.SelectMessageFile();
            if (messageImageFile == null)
            {
                return;
            }

            if (messageImageFile.FileType == ".bmp" || messageImageFile.FileType == ".png")
            {
                this.messageImageFile = messageImageFile;
                var bitmapImage = await this.convertToBitmap(messageImageFile);
                this.monochromeImageDisplay.Source = bitmapImage;
            }
            else
            {
                //TODO handle loading text file
                //load text from file into text area
            }

            //TODO enable settings if source and message are loaded, not needed for demo
            
            if (this.sourceImageFile != null && this.messageImageFile != null)
            {
                this.enableSettingsOptions();
            }
        }

        private void enableSettingsOptions()
        {
            this.embedButton.IsEnabled = true;
            this.encryptionSelectionCheckBox.IsEnabled = true;
            this.BPCCSelectionComboBox.IsEnabled = true;
        }

        private async void embedButton_Click(object sender, RoutedEventArgs e)
        {
            var sourceDecoder = await BitmapDecoder.CreateAsync(await this.sourceImageFile.OpenAsync(FileAccessMode.Read));
            var sourcePixels = await this.extractPixelDataFromFile(this.sourceImageFile);

            await this.embedMessageInImage(sourcePixels, sourceDecoder.PixelWidth, sourceDecoder.PixelHeight);

            if (this.messageImageTooLarge)
            {
                return;
            }

            this.embeddedImage = new WriteableBitmap((int)sourceDecoder.PixelWidth, (int)sourceDecoder.PixelHeight);
            using (var writeStream = this.embeddedImage.PixelBuffer.AsStream())
            {
                await writeStream.WriteAsync(sourcePixels, 0, sourcePixels.Length);
                this.embeddedImageDisplay.Source = this.embeddedImage;
            }

            this.saveButton.IsEnabled = true;
        }

        private void homeButton_click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            this.fileWriter.SaveWritableBitmap(this.embeddedImage, this.dpiX, this.dpiY);
        }

        private async Task embedMessageInImage(byte[] sourcePixels, uint imageWidth, uint imageHeight)
        {
            var messagePixels = await this.extractPixelDataFromFile(this.messageImageFile);

            var messageDecoder = await BitmapDecoder.CreateAsync(await this.messageImageFile.OpenAsync(FileAccessMode.Read));
            var messageImageWidth = messageDecoder.PixelWidth;
            var messageImageHeight = messageDecoder.PixelHeight;

            if (messageImageWidth > imageWidth || messageImageHeight > imageHeight)
            {
                await this.showMessageFileTooLargeDialog();
                this.messageImageTooLarge = true;
                return;
            }

            for (var currY = 0; currY < imageHeight; currY++)
            {
                for (var currX = 0; currX < imageWidth; currX++)
                {
                    var sourcePixelColor = this.GetPixelBgra8(sourcePixels, currY, currX, imageWidth, imageHeight);

                    if (currY == 0 && currX == 0)
                    {
                        sourcePixelColor.R = 212;
                        sourcePixelColor.G = 212;
                        sourcePixelColor.B = 212;
                    }
                    else if (currY == 0 && currX == 1)
                    {
                        //handle encryption selection
                        var isChecked = this.encryptionSelectionCheckBox.IsChecked;
                        if (isChecked != null && (bool)isChecked)
                        {
                            sourcePixelColor.R |= 1; //set LSB red source pixel to 1
                        }
                        else
                        {
                            sourcePixelColor.R &= 0xfe; //set LSB red source pixel to 0
                        }
                        //handle combobox selection
                        var selectedBPCC = (ComboBoxItem)this.BPCCSelectionComboBox.SelectedItem;
                        if (selectedBPCC != null)
                        {
                            var bpcc = int.Parse(selectedBPCC.Content.ToString()); // pull selected bpcc from UI
                            var binaryBpcc = this.calculateBinaryForBPCC(bpcc); // convert bpcc to binary representation
                            sourcePixelColor.G = (byte)binaryBpcc; // set the green channel to binary representation of bpcc selection
                        }
                        //handle embedding type
                        if (this.messageImageFile.FileType.Equals(".txt"))
                        {
                            sourcePixelColor.B |= 1; //set LSB blue source pixel to 1
                        }
                        else
                        {
                            sourcePixelColor.B &= 0xfe; //set LSB blue source pixel to 0
                        }
                        
                    }
                    else
                    {
                        if (currX < messageImageWidth && currY < messageImageHeight)
                        {
                            var messagePixelColor = this.GetPixelBgra8(messagePixels, currY,
                                                            currX, messageImageWidth, messageImageHeight);

                            if (messagePixelColor.R == 0 && messagePixelColor.B == 0 && messagePixelColor.G == 0)
                            {
                                sourcePixelColor.B &= 0xfe; //set LSB blue source pixel to 0
                            } 
                            else if (messagePixelColor.R == 255 && messagePixelColor.B == 255 && messagePixelColor.G == 255)
                            {
                                sourcePixelColor.B |= 1; //set LSB blue source pixel to 1
                            }
                        }
                    }
                    
                    this.SetPixelBgra8(sourcePixels, currY, currX, sourcePixelColor, imageWidth, imageHeight);
                }
            }
        }

        private async Task showMessageFileTooLargeDialog()
        {
            ContentDialog messageFileTooLargeDialog = new ContentDialog()
            {
                Title = "ERROR",
                Content = "Message file exceeds the dimensions of the source image" 
                          + Environment.NewLine + "Embedding will not occur" 
                          + Environment.NewLine + "Choose another source or message image and try again.",
                CloseButtonText = "Ok"
            };

            await messageFileTooLargeDialog.ShowAsync();
        }

        private int calculateBinaryForBPCC(int bpccSelection)
        {
            var sum = 0.0;
            for (var i = 0; i < bpccSelection; i++)
            {
                sum += Math.Pow(2, i);
            }
            return Convert.ToInt32(sum);
        }

        private async Task<BitmapImage> convertToBitmap(StorageFile imageFile)
        {
            IRandomAccessStream inputStream = await imageFile.OpenReadAsync();
            var newImage = new BitmapImage();
            newImage.SetSource(inputStream);
            return newImage;
        }

        /// <summary>
        ///     Gets the pixel bgra8 color from the current pixel.
        /// </summary>
        /// <param name="pixels">The pixels.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <returns>The color of the current pixel</returns>
        public Color GetPixelBgra8(byte[] pixels, int x, int y, uint width, uint height)
        {
            var offset = (x * (int)width + y) * 4;
            var r = pixels[offset + 2];
            var g = pixels[offset + 1];
            var b = pixels[offset + 0];
            return Color.FromArgb(0, r, g, b);
        }

        /// <summary>
        ///     Sets the pixel bgra8 color to the current pixel.
        /// </summary>
        /// <param name="pixels">The pixels.</param>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <param name="color">The color.</param>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        public void SetPixelBgra8(byte[] pixels, int x, int y, Color color, uint width, uint height)
        {
            var offset = (x * (int)width + y) * 4;
            pixels[offset + 2] = color.R;
            pixels[offset + 1] = color.G;
            pixels[offset + 0] = color.B;
        }

        #endregion

        private async Task<byte[]> extractPixelDataFromFile(StorageFile file)
        {
            var copyBitmapImage = await this.convertToBitmap(file);

            using (var fileStream = await file.OpenAsync(FileAccessMode.Read))
            {
                var decoder = await BitmapDecoder.CreateAsync(fileStream);
                var transform = new BitmapTransform {
                    ScaledWidth = Convert.ToUInt32(copyBitmapImage.PixelWidth),
                    ScaledHeight = Convert.ToUInt32(copyBitmapImage.PixelHeight)
                };

                if (file.Path == this.sourceImageFile.Path)
                {
                    this.dpiX = decoder.DpiX;
                    this.dpiY = decoder.DpiY;
                }

                var pixelData = await decoder.GetPixelDataAsync(
                    BitmapPixelFormat.Bgra8,
                    BitmapAlphaMode.Straight,
                    transform,
                    ExifOrientationMode.IgnoreExifOrientation,
                    ColorManagementMode.DoNotColorManage
                );

                return pixelData.DetachPixelData();
            }
        }
    }
}
