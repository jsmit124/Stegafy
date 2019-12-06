using System;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;
using GroupNStegafy.Controller;
using GroupNStegafy.Utility;

namespace GroupNStegafy.View
{
    /// <summary>
    ///     An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ExtractMessagePage
    {
        #region Data members

        private readonly double applicationHeight = (double) Application.Current.Resources["AppHeight"];
        private readonly double applicationWidth = (double) Application.Current.Resources["AppWidth"];

        private double dpiX;
        private double dpiY;

        private WriteableBitmap extractedImage;
        private StorageFile embeddedImageFile;

        private readonly ExtractManager extractManager;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExtractMessagePage" /> class.
        /// </summary>
        public ExtractMessagePage()
        {
            this.InitializeComponent();

            ApplicationView.PreferredLaunchViewSize = new Size
                {Width = this.applicationWidth, Height = this.applicationHeight};
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            ApplicationView.GetForCurrentView()
                           .SetPreferredMinSize(new Size(this.applicationWidth, this.applicationHeight));

            this.extractManager = new ExtractManager();

            this.extractedImage = null;
            this.embeddedImageFile = null;

            this.dpiX = 0;
            this.dpiY = 0;
        }

        #endregion

        #region Methods

        private void homeButton_click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage), null, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromLeft });
        }

        private async void loadEmbeddedImageButton_Click(object sender, RoutedEventArgs e)
        {
            await this.extractManager.LoadEmbeddedImage();

            this.embeddedImageDisplay.Source = this.extractManager.EmbeddedImage;

            if (this.extractManager.EmbeddedImageFile != null)
            {
                this.extractButton.IsEnabled = true;
            }
        }

        private async void extractButton_Click(object sender, RoutedEventArgs e)
        {
            var embeddedDecoder =
                await BitmapDecoder.CreateAsync(await this.embeddedImageFile.OpenAsync(FileAccessMode.Read));
            var embeddedPixels = await PixelExtracter.ExtractPixelDataFromFile(this.embeddedImageFile);

            //await this.extractMessageFromImage(embeddedPixels, embeddedDecoder.PixelWidth, embeddedDecoder.PixelHeight);

            this.extractedImage =
                new WriteableBitmap((int) embeddedDecoder.PixelWidth, (int) embeddedDecoder.PixelHeight);
            using (var writeStream = this.extractedImage.PixelBuffer.AsStream())
            {
                await writeStream.WriteAsync(embeddedPixels, 0, embeddedPixels.Length);
                this.decryptedImageDisplay.Source = this.extractedImage;
            }

            this.saveDecryptedMessageButton.IsEnabled = true;
        }

        private void saveDecryptedMessageButton_Click(object sender, RoutedEventArgs e)
        {
            this.extractManager.SaveExtractedMessage(this.extractedImage, this.dpiX, this.dpiY);
        }

        #endregion
    }
}