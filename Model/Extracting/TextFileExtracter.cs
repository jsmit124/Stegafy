using System;
using System.Collections;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;
using GroupNStegafy.Converter;
using GroupNStegafy.Cryptography;
using GroupNStegafy.Utility;
using GroupNStegafy.View;
using Windows.UI;

namespace GroupNStegafy.Model.Extracting
{
    public class TextFileExtracter : MessageExtracter
    {
        private int bpcc;
        private BitArray embeddedBits;

        public override async Task ExtractMessageFromImage(byte[] embeddedPixels, uint embeddedImageWidth,
           uint embeddedImageHeight)
        {
            var encryptionPassword = new StringBuilder();
            var embeddedText = new StringBuilder();
            var count = 0;

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

                        var numberOfBits = ((embeddedImageWidth * embeddedImageHeight) - 2) * this.bpcc;
                        this.embeddedBits = new BitArray((int)numberOfBits);
                    }
                    else
                    {
                        //TODO extract text information
                        this.extractMessageBitsFromPixel(embeddedPixelColor, count);
                        count += 3 * this.bpcc;
                    }
                }
            }

            if (this.EncryptionUsed)
            {
                this.DecryptedText = TextCryptography.Decrypt(encryptionPassword.ToString(), embeddedText.ToString());
            }

            this.ExtractedText = embeddedText.ToString();
        }

        private void extractMessageBitsFromPixel(Color embeddedPixelColor, int count)
        {
            //var extractedBits = new BitArray(this.bpcc * 3);

            foreach (var i in Enumerable.Range(0, 3))
            {
                byte color;
                if (i == 0)
                {
                    color = embeddedPixelColor.R;
                }
                else if (i == 1)
                {
                    color = embeddedPixelColor.G;
                }
                else
                {
                    color = embeddedPixelColor.B;
                }

                var colorBits = new BitArray(color);

                for (var j = 7; i > this.bpcc; j++)
                {
                    this.embeddedBits.Set(count, colorBits.Get(j));
                    count++;
                }
            }
        }
    }
}
