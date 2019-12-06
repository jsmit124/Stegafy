using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI;
using GroupNStegafy.Enumerables;
using GroupNStegafy.Formatter;
using GroupNStegafy.Utility;
using GroupNStegafy.View;

namespace GroupNStegafy.Model
{
    /// <summary>
    ///     Stores information for the TextMessageEmbedder class
    /// </summary>
    /// <seealso cref="GroupNStegafy.Model.Embedder" />
    public class TextMessageEmbedder : Embedder
    {
        #region Methods

        /// <summary>
        ///     Embeds the text message in image.
        /// </summary>
        /// <param name="messageData">The message data.</param>
        /// <param name="messageLength">Length of the message.</param>
        /// <param name="messageImageHeight">Height of the message image.</param>
        /// <param name="sourceImageWidth">Width of the source image.</param>
        /// <param name="sourceImageHeight">Height of the source image.</param>
        /// <param name="encryptionIsChecked">if set to <c>true</c> [encryption is selected].</param>
        /// <param name="bpcc">The BPCC.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override async Task EmbedMessageInImage(byte[] messageData, uint messageLength, uint messageImageHeight,
            uint sourceImageWidth, uint sourceImageHeight, bool encryptionIsChecked, int bpcc)
        {
            var messageBits = new BitArray(messageData);
            var totalAvailableSourcePixels = sourceImageWidth * sourceImageHeight - 2;

            if (messageBits.Count / bpcc > totalAvailableSourcePixels * 3)
            {
                var requiredBpcc = this.calculateBpccRequiredToEmbedText(messageBits.Count, totalAvailableSourcePixels);
                if (requiredBpcc > 8)
                {
                    await Dialogs.ShowNotPossibleToEmbedTextDialog();
                }
                else
                {
                    await Dialogs.ShowRequiredBpccToEmbedTextDialog(requiredBpcc);
                }

                MessageTooLarge = true;
                return;
            }

            var currentIndex = 0;

            for (var currY = 0; currY < sourceImageHeight; currY++)
            {
                for (var currX = 0; currX < sourceImageWidth; currX++)
                {
                    var sourcePixelColor =
                        PixelColorInfo.GetPixelBgra8(SourceImagePixels, currY, currX, sourceImageWidth);

                    if (IsFirstPixel(currX, currY))
                    {
                        sourcePixelColor = HeaderPixelFormatter.FormatFirstHeaderPixel(sourcePixelColor);
                    }
                    else if (IsSecondPixel(currX, currY))
                    {
                        sourcePixelColor = HeaderPixelFormatter.FormatSecondHeaderPixel(FileTypes.Text,
                            sourcePixelColor, encryptionIsChecked, bpcc);
                    }
                    else
                    {
                        sourcePixelColor = this.embedTextMessage(messageBits, sourcePixelColor, currentIndex, bpcc);
                        currentIndex += 3 * bpcc;
                    }

                    PixelColorInfo.SetPixelBgra8(SourceImagePixels, currY, currX, sourcePixelColor, sourceImageWidth);
                }
            }

            await SetEmbeddedImage(sourceImageHeight, sourceImageWidth);
        }

        private Color embedTextMessage(BitArray messageBits, Color sourcePixelColor, int currentIndex, int bpcc)
        {
            foreach (var i in Enumerable.Range(0, 3))
            {
                if (currentIndex < messageBits.Count) // if the current index is less than the amount of bits in the text, continue
                {
                    int leadingRemoved;
                    byte color;

                    if (i == 0)
                    {
                        color = sourcePixelColor.R;
                    }
                    else if (i == 1)
                    {
                        color = sourcePixelColor.G;
                    }
                    else
                    {
                        color = sourcePixelColor.B;
                    }

                    leadingRemoved = color >> bpcc; // remove leading bpcc amount of bits
                    leadingRemoved <<= bpcc; // add leading bpcc amount of bits as zeros

                    var bitsToAdd = new BitArray(8); // create temp bit array
                    for (var j = 0; j < bpcc; j++) // 
                    {
                        if (currentIndex + j < messageBits.Count)
                        {
                            bitsToAdd.Set(j, messageBits[currentIndex + j]);
                        }
                        else
                        {
                            bitsToAdd.Set(j, false);
                        }
                    }

                    var bitsAsByte = new byte[1]; //create empty byte
                    bitsToAdd.CopyTo(bitsAsByte, 0); //set byte to specified number of bits

                    leadingRemoved |= bitsAsByte[0];
                    color = (byte) leadingRemoved;

                    if (i == 0)
                    {
                        sourcePixelColor.R = color;
                    }
                    else if (i == 1)
                    {
                        sourcePixelColor.G = color;
                    }
                    else
                    {
                        sourcePixelColor.B = color;
                    }

                    currentIndex += bpcc;
                }

            }

            return sourcePixelColor;
        }

        private int calculateBpccRequiredToEmbedText(int bitCount, uint totalSourcePixels)
        {
            var requiredBpcc = 1;

            for (var i = 1; bitCount / i > totalSourcePixels * 3; i++)
            {
                requiredBpcc++;
            }

            return requiredBpcc;
        }

        #endregion
    }
}