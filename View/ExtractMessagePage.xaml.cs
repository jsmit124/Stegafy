﻿using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
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

        private readonly double applicationHeight = (double) Application.Current.Resources["AppHeight"];
        private readonly double applicationWidth = (double) Application.Current.Resources["AppWidth"];

        private double dpiX;
        private double dpiY;
        private WriteableBitmap extractedImage;
        private StorageFile embeddedImageFile;
        private readonly FileWriter fileWriter;
        private readonly FileReader fileReader;


        /// <summary>
        /// Initializes a new instance of the <see cref="ExtractMessagePage"/> class.
        /// </summary>
        public ExtractMessagePage()
        {
            this.InitializeComponent();

            ApplicationView.PreferredLaunchViewSize = new Size
                {Width = this.applicationWidth, Height = this.applicationHeight};
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            ApplicationView.GetForCurrentView()
                .SetPreferredMinSize(new Size(this.applicationWidth, this.applicationHeight));

            this.extractedImage = null;
            this.embeddedImageFile = null;

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
            this.embeddedImageFile = await this.fileReader.SelectSourceImageFile();
            if (this.embeddedImageFile == null)
            {
                return;
            }

            var copyBitmapImage = await this.convertToBitmap(this.embeddedImageFile);
            this.embeddedImageDisplay.Source = copyBitmapImage;

            if (this.embeddedImageFile != null)
            {
                this.extractButton.IsEnabled = true;
            }
        }

        private async void extractButton_Click(object sender, RoutedEventArgs e)
        {
            var embeddedDecoder = await BitmapDecoder.CreateAsync(await this.embeddedImageFile.OpenAsync(FileAccessMode.Read));
            var embeddedPixels = await this.extractPixelDataFromFile(this.embeddedImageFile);

            await this.extractMessageFromImage(embeddedPixels, embeddedDecoder.PixelWidth, embeddedDecoder.PixelHeight);
           
            this.extractedImage = new WriteableBitmap((int) embeddedDecoder.PixelWidth, (int) embeddedDecoder.PixelHeight);
            using (var writeStream = this.extractedImage.PixelBuffer.AsStream())
            {
                await writeStream.WriteAsync(embeddedPixels, 0, embeddedPixels.Length);
                this.decryptedImageDisplay.Source = this.extractedImage;
            }

            this.saveDecryptedMessageButton.IsEnabled = true;
        }

        private void saveDecryptedMessageButton_Click(object sender, RoutedEventArgs e)
        {
            this.fileWriter.SaveWritableBitmap(this.extractedImage, this.dpiX, this.dpiY);
        }

        private async Task<BitmapImage> convertToBitmap(StorageFile imageFile)
        {
            IRandomAccessStream inputStream = await imageFile.OpenReadAsync();
            var newImage = new BitmapImage();
            newImage.SetSource(inputStream);
            return newImage;
        }

        private static bool isBitSet(byte b, int pos)
        {
            return (b & (1 << pos)) != 0;
        }

        private async Task<byte[]> extractPixelDataFromFile(StorageFile file)
        {
            var copyBitmapImage = await this.convertToBitmap(file);

            using (var fileStream = await file.OpenAsync(FileAccessMode.Read))
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

                return pixelData.DetachPixelData();
            }
        }

        private async Task extractMessageFromImage(byte[] embeddedPixels, uint embeddedImageWidth, uint embeddedImageHeight)
        {
            for (var currY = 0; currY < embeddedImageHeight; currY++)
            {
                for (var currX = 0; currX < embeddedImageWidth; currX++)
                {
                    var embeddedPixelColor = this.GetPixelBgra8(embeddedPixels, currY, currX, embeddedImageWidth,
                        embeddedImageHeight);

                    if (currY == 0 && currX == 0)
                    {
                        if (!(embeddedPixelColor.R == 212 && embeddedPixelColor.B == 212 && embeddedPixelColor.G == 212))
                        {
                            //TODO handle no message embedded in the picture, not needed for demo
                            return;
                        }
                    }
                    else if (currY == 0 && currX == 1)
                    {
                        //TODO Configure message extraction settings and whatnot based on the values stores in the RGB bytes, not needed for demo
                    }
                    //TODO Check for message stop symbol
                    else
                    {
                        var currentBlueColorByte = embeddedPixelColor.B;
                        if (isBitSet(currentBlueColorByte, 0))
                        {
                            embeddedPixelColor.R = 255;
                            embeddedPixelColor.B = 255;
                            embeddedPixelColor.G = 255;
                        }
                        else
                        {
                            embeddedPixelColor.R = 0;
                            embeddedPixelColor.B = 0;
                            embeddedPixelColor.G = 0;
                        }
                    }

                    this.SetPixelBgra8(embeddedPixels, currY, currX, embeddedPixelColor, embeddedImageWidth, embeddedImageHeight);
                }
            }

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
    }
}
