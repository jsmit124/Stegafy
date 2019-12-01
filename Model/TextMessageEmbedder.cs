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
                        sourcePixelColor = this.embedTextMessage(messageBits, sourcePixelColor, currentIndex);
                        currentIndex += 3;
                    }

                    PixelColorInfo.SetPixelBgra8(SourceImagePixels, currY, currX, sourcePixelColor, sourceImageWidth);
                }
            }

            await SetEmbeddedImage(sourceImageHeight, sourceImageWidth);
        }

        private Color embedTextMessage(BitArray messageBits, Color sourcePixelColor, int currentIndex)
        {
            foreach (var i in Enumerable.Range(0, 3))
            {
                currentIndex += i;

                if (currentIndex < messageBits.Count
                ) // if the current index is less than the amount of bits in the text, continue
                {
                    var messageBit = messageBits.Get(currentIndex); // current bit

                    if (messageBit)
                    {
                        if ((i + 1) % 3 == 0)
                        {
                            sourcePixelColor.B |= 1; //set LSB blue source pixel to 1
                        }
                        else if ((i + 1) % 2 == 0)
                        {
                            sourcePixelColor.G |= 1; //set LSB green source pixel to 1
                        }
                        else
                        {
                            sourcePixelColor.R |= 1; //set LSB red source pixel to 1
                        }
                    }
                    else
                    {
                        if ((i + 1) % 3 == 0)
                        {
                            sourcePixelColor.B &= 0xfe; //set LSB blue source pixel to 0
                        }
                        else if ((i + 1) % 2 == 0)
                        {
                            sourcePixelColor.G &= 0xfe; //set LSB green source pixel to 0
                        }
                        else
                        {
                            sourcePixelColor.R &= 0xfe; //set LSB red source pixel to 0
                        }
                    }
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