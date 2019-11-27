using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using GroupNStegafy.Converter;
using GroupNStegafy.Formatter;
using GroupNStegafy.IO;
using GroupNStegafy.Utility;

namespace GroupNStegafy.View
{
    /// <summary>
    ///     The embedding page used for embedding images
    /// </summary>
    public sealed partial class EmbedMessagePage : Page
    {
        #region Data members

        private readonly double applicationHeight = (double) Application.Current.Resources["AppHeight"];
        private readonly double applicationWidth = (double) Application.Current.Resources["AppWidth"];

        private double dpiX;
        private double dpiY;
        private WriteableBitmap embeddedImage;
        private StorageFile messageFile;
        private StorageFile sourceImageFile;
        private readonly FileWriter fileWriter;
        private readonly FileReader fileReader;
        private bool messageImageTooLarge;

        private const string EndOfTextFileIndication = "#.-.-. -#";

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="EmbedMessagePage" /> class.
        /// </summary>
        public EmbedMessagePage()
        {
            this.InitializeComponent();

            ApplicationView.PreferredLaunchViewSize = new Size
                {Width = this.applicationWidth, Height = this.applicationHeight};
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            ApplicationView.GetForCurrentView()
                           .SetPreferredMinSize(new Size(this.applicationWidth, this.applicationHeight));

            this.sourceImageFile = null;
            this.messageFile = null;
            this.embeddedImage = null;
            this.messageImageTooLarge = false;

            this.dpiX = 0;
            this.dpiY = 0;

            this.fileWriter = new FileWriter();
            this.fileReader = new FileReader();
        }

        #endregion

        #region keep these methods in code behind

        private async void loadSourceButton_Click(object sender, RoutedEventArgs e)
        {
            this.sourceImageFile = await this.fileReader.SelectSourceImageFile();
            if (this.sourceImageFile == null)
            {
                return;
            }

            var sourceImage = await FileBitmapConverter.ConvertFileToBitmap(this.sourceImageFile);
            this.sourceImageDisplay.Source = sourceImage;

            this.checkIfBothDisplaysLoadedToEnableSettings();
        }

        private async void loadMessageButton_Click(object sender, RoutedEventArgs e)
        {
            var messageImageFile = await this.fileReader.SelectMessageFile();
            if (messageImageFile == null)
            {
                return;
            }

            this.hideMessageDisplays();
            this.messageFile = messageImageFile;

            if (messageImageFile.FileType == ".bmp" || messageImageFile.FileType == ".png")
            {
                var bitmapImage = await FileBitmapConverter.ConvertFileToBitmap(messageImageFile);
                this.monochromeImageDisplay.Source = bitmapImage;
                this.monochromeImageDisplay.Visibility = Visibility.Visible;
            }
            else
            {
                var textFromFile = await this.fileReader.ReadTextFromFile(this.messageFile);
                this.textFileDisplay.Text = textFromFile;
                this.textFileDisplay.Visibility = Visibility.Visible;
            }

            this.checkIfBothDisplaysLoadedToEnableSettings();
        }

        private async void embedButton_Click(object sender, RoutedEventArgs e)
        {
            this.embeddingProgressRing.IsActive = true;

            var sourceDecoder =
                await BitmapDecoder.CreateAsync(await this.sourceImageFile.OpenAsync(FileAccessMode.Read));
            var sourcePixels = await this.extractPixelDataFromFile(this.sourceImageFile);

            await this.embedMessageInImage(sourcePixels, sourceDecoder.PixelWidth, sourceDecoder.PixelHeight);

            if (this.messageImageTooLarge)
            {
                this.embeddingProgressRing.IsActive = false;
                return;
            }

            this.embeddedImage = new WriteableBitmap((int) sourceDecoder.PixelWidth, (int) sourceDecoder.PixelHeight);
            using (var writeStream = this.embeddedImage.PixelBuffer.AsStream())
            {
                await writeStream.WriteAsync(sourcePixels, 0, sourcePixels.Length);
                this.embeddedImageDisplay.Source = this.embeddedImage;
            }

            this.saveButton.IsEnabled = true;
            this.embeddingProgressRing.IsActive = false;
        }

        private void homeButton_click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage), null, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromRight });
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            this.fileWriter.SaveWritableBitmap(this.embeddedImage, this.dpiX, this.dpiY);
        }

        private void checkIfBothDisplaysLoadedToEnableSettings()
        {
            if (this.sourceImageFile != null && this.messageFile != null)
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

        private void hideMessageDisplays()
        {
            this.monochromeImageDisplay.Visibility = Visibility.Collapsed;
            this.textFileDisplay.Visibility = Visibility.Collapsed;
        }

        #endregion

        #region methods to abstract out

        private async Task embedMessageInImage(byte[] sourcePixels, uint sourceImageWidth, uint sourceImageHeight)
        {
            var messagePixels = await this.extractPixelDataFromFile(this.messageFile);

            var messageDecoder =
                await BitmapDecoder.CreateAsync(await this.messageFile.OpenAsync(FileAccessMode.Read));
            var messageImageWidth = messageDecoder.PixelWidth;
            var messageImageHeight = messageDecoder.PixelHeight;

            if (messageImageWidth > sourceImageWidth || messageImageHeight > sourceImageHeight)
            {
                await Dialogs.ShowMessageFileTooLargeDialog();
                this.messageImageTooLarge = true;
                return;
            }

            for (var currY = 0; currY < sourceImageHeight; currY++)
            {
                for (var currX = 0; currX < sourceImageWidth; currX++)
                {
                    var sourcePixelColor =
                        PixelColorInfo.GetPixelBgra8(sourcePixels, currY, currX, sourceImageWidth, sourceImageHeight);

                    if (isFirstPixel(currX, currY))
                    {
                        sourcePixelColor = HeaderPixelFormatter.FormatFirstHeaderPixel(sourcePixelColor);
                    }
                    else if (isSecondPixel(currX, currY))
                    {
                        var encryptionIsChecked = this.encryptionSelectionCheckBox.IsChecked.Value;
                        var bpccSelection = (ComboBoxItem) this.BPCCSelectionComboBox.SelectedItem;
                        var bpcc = int.Parse(bpccSelection.Content.ToString());

                        sourcePixelColor = HeaderPixelFormatter.FormatSecondHeaderPixel(this.messageFile,
                            sourcePixelColor, encryptionIsChecked, bpcc);
                    }
                    else if (this.messageFile.FileType == ".txt")
                    {
                        //TODO complete text embedding
                    }
                    else
                    {
                        sourcePixelColor = this.embedMonochromeImage(currX, messageImageWidth, currY,
                            messageImageHeight, messagePixels, sourcePixelColor);
                    }

                    PixelColorInfo.SetPixelBgra8(sourcePixels, currY, currX, sourcePixelColor, sourceImageWidth,
                        sourceImageHeight);
                }
            }
        }

        private Color embedMonochromeImage(int currX, uint messageImageWidth, int currY, uint messageImageHeight,
            byte[] messagePixels, Color sourcePixelColor)
        {
            if (currX < messageImageWidth && currY < messageImageHeight)
            {
                var messagePixelColor = PixelColorInfo.GetPixelBgra8(messagePixels, currY,
                    currX, messageImageWidth, messageImageHeight);

                if (isBlackPixel(messagePixelColor))
                {
                    sourcePixelColor.B &= 0xfe; //set LSB blue source pixel to 0
                }
                else if (isWhitePixel(messagePixelColor))
                {
                    sourcePixelColor.B |= 1; //set LSB blue source pixel to 1
                }
            }

            return sourcePixelColor;
        }

        private async Task<byte[]> extractPixelDataFromFile(StorageFile file)
        {
            var copyBitmapImage = await FileBitmapConverter.ConvertFileToBitmap(file);

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

        private static bool isFirstPixel(int currX, int currY)
        {
            return currY == 0 && currX == 0;
        }

        private static bool isSecondPixel(int currX, int currY)
        {
            return currY == 0 && currX == 1;
        }

        private static bool isWhitePixel(Color messagePixelColor)
        {
            return messagePixelColor.R == 255 && messagePixelColor.B == 255 &&
                   messagePixelColor.G == 255;
        }

        private static bool isBlackPixel(Color messagePixelColor)
        {
            return messagePixelColor.R == 0 && messagePixelColor.B == 0 && messagePixelColor.G == 0;
        }

        #endregion
    }
}