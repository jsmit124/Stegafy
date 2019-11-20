using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Imaging;
using GroupNStegafy.IO;

namespace GroupNStegafy.View
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ExtractMessagePage
    {

        private readonly double applicationHeight = (double)Application.Current.Resources["AppHeight"];
        private readonly double applicationWidth = (double)Application.Current.Resources["AppWidth"];

        private double dpiX;
        private double dpiY;
        private WriteableBitmap modifiedImage;
        private readonly FileWriter fileWriter;
        private readonly FileReader fileReader;


        /// <summary>
        /// Initializes a new instance of the <see cref="ExtractMessagePage"/> class.
        /// </summary>
        public ExtractMessagePage()
        {
            this.InitializeComponent();

            ApplicationView.PreferredLaunchViewSize = new Size
                { Width = this.applicationWidth, Height = this.applicationHeight };
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            ApplicationView.GetForCurrentView()
                           .SetPreferredMinSize(new Size(this.applicationWidth, this.applicationHeight));

            this.modifiedImage = null;
            this.dpiX = 0;
            this.dpiY = 0;

            this.fileWriter = new FileWriter();
            this.fileReader = new FileReader();
        }

        private void homeButton_click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        private async void loadEmbeddedImageButton_Click(object sender, RoutedEventArgs e)
        {
            var sourceImageFile = await this.fileReader.SelectSourceImageFile();
            var copyBitmapImage = await makeACopyOfTheFileToWorkOn(sourceImageFile);

            using (var fileStream = await sourceImageFile.OpenAsync(FileAccessMode.Read))
            {
                var decoder = await BitmapDecoder.CreateAsync(fileStream);
                var transform = new BitmapTransform
                {
                    ScaledWidth = Convert.ToUInt32(copyBitmapImage.PixelWidth),
                    ScaledHeight = Convert.ToUInt32(copyBitmapImage.PixelHeight)
                };

                this.dpiX = decoder.DpiX;
                this.dpiY = decoder.DpiY;

                var pixelData = await decoder.GetPixelDataAsync(
                    BitmapPixelFormat.Bgra8,
                    BitmapAlphaMode.Straight,
                    transform,
                    ExifOrientationMode.IgnoreExifOrientation,
                    ColorManagementMode.DoNotColorManage
                );

                var sourcePixels = pixelData.DetachPixelData();


                this.modifiedImage = new WriteableBitmap((int)decoder.PixelWidth, (int)decoder.PixelHeight);
                using (var writeStream = this.modifiedImage.PixelBuffer.AsStream())
                {
                    await writeStream.WriteAsync(sourcePixels, 0, sourcePixels.Length);
                    this.embeddedImageDisplay.Source = this.modifiedImage;
                }
            }

        }

        private static async Task<BitmapImage> makeACopyOfTheFileToWorkOn(StorageFile imageFile)
        {
            IRandomAccessStream inputStream = await imageFile.OpenReadAsync();
            var newImage = new BitmapImage();
            newImage.SetSource(inputStream);
            return newImage;
        }

        private void saveDecryptedMessageButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
