using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media.Imaging;
using GroupNStegafy.Utility;
using GroupNStegafy.View;

namespace GroupNStegafy.Model.Extracting
{
    /// <summary>
    ///     Stores information for the MonochromeImageExtractor class
    /// </summary>
    /// <seealso cref="GroupNStegafy.Model.Extracting.MessageExtracter" />
    public class MonochromeImageExtracter : MessageExtracter
    {
        #region Data members

        private readonly Color whitePixel = Color.FromArgb(255, 255, 255, 255);
        private readonly Color blackPixel = Color.FromArgb(255, 0, 0, 0);

        #endregion

        #region Methods

        /// <summary>
        ///     Extracts the message from image.
        /// </summary>
        /// @Precondition none
        /// @postcondition message is extracted from the image
        /// <param name="embeddedPixels">The embedded pixels.</param>
        /// <param name="embeddedImageWidth">Width of the embedded image.</param>
        /// <param name="embeddedImageHeight">Height of the embedded image.</param>
        public override async Task ExtractMessageFromImage(byte[] embeddedPixels, uint embeddedImageWidth,
            uint embeddedImageHeight)
        {
            for (var currY = 0; currY < embeddedImageHeight; currY++)
            {
                for (var currX = 0; currX < embeddedImageWidth; currX++)
                {
                    var embeddedPixelColor = PixelColorInfo.GetPixelBgra8(embeddedPixels, currY, currX,
                        embeddedImageWidth);

                    if (isFirstPixel(currY, currX))
                    {
                        if (!(embeddedPixelColor.R == 212 && embeddedPixelColor.B == 212 &&
                              embeddedPixelColor.G == 212))
                        {
                            await Dialogs.ShowNoMessageDialog();
                            return;
                        }
                    }
                    else if (isSecondPixel(currY, currX))
                    {
                        EncryptionUsed = (embeddedPixelColor.R & 1) == 0;
                    }
                    else
                    {
                        var currentBlueColorByte = embeddedPixelColor.B;
                        if (isBitSet(currentBlueColorByte, 0))
                        {
                            embeddedPixelColor = this.whitePixel;
                        }
                        else
                        {
                            embeddedPixelColor = this.blackPixel;
                        }
                    }

                    PixelColorInfo.SetPixelBgra8(embeddedPixels, currY, currX, embeddedPixelColor, embeddedImageWidth);
                }
            }

            ExtractedImage =
                new WriteableBitmap((int) embeddedImageWidth, (int) embeddedImageHeight);
            using (var writeStream = ExtractedImage.PixelBuffer.AsStream())
            {
                await writeStream.WriteAsync(embeddedPixels, 0, embeddedPixels.Length);
            }
        }

        private static bool isBitSet(byte bits, int pos)
        {
            return (bits & (1 << pos)) != 0;
        }

        #endregion
    }
}