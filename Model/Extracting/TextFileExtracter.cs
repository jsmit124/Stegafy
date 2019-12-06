using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using GroupNStegafy.Converter;
using GroupNStegafy.Utility;
using GroupNStegafy.View;

namespace GroupNStegafy.Model.Extracting
{
    public class TextFileExtracter : MessageExtracter
    {
        private int bpcc;

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
                        this.EncryptionUsed = (embeddedPixelColor.R & 1) != 0;
                        this.bpcc = BinaryDecimalConverter.CalculateBpccFromBinary(embeddedPixelColor.G);
                    }
                    else
                    {
                        //TODO extract text information
                    }

                    PixelColorInfo.SetPixelBgra8(embeddedPixels, currY, currX, embeddedPixelColor, embeddedImageWidth);
                }
            }

            this.ExtractedImage =
                new WriteableBitmap((int)embeddedImageWidth, (int)embeddedImageHeight);
            using (var writeStream = this.ExtractedImage.PixelBuffer.AsStream())
            {
                await writeStream.WriteAsync(embeddedPixels, 0, embeddedPixels.Length);
            }
        }
    }
}
