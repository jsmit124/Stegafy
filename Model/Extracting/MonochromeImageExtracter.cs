using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Xaml.Media.Imaging;
using GroupNStegafy.Utility;
using GroupNStegafy.View;

namespace GroupNStegafy.Model.Extracting
{
    public class MonochromeImageExtracter : MessageExtracter
    {
        private readonly Color whitePixel = Color.FromArgb(255, 255, 255, 255);
        private readonly Color blackPixel = Color.FromArgb(255, 0, 0, 0);

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
                        //TODO Configure message extraction settings and whatnot based on the values stores in the RGB bytes, not needed for demo
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

            this.ExtractedMessage =
                new WriteableBitmap((int)embeddedImageWidth, (int)embeddedImageHeight);
            using (var writeStream = this.ExtractedMessage.PixelBuffer.AsStream())
            {
                await writeStream.WriteAsync(embeddedPixels, 0, embeddedPixels.Length);
            }
        }

        private static bool isBitSet(byte b, int pos)
        {
            return (b & (1 << pos)) != 0;
        }
    }
}
