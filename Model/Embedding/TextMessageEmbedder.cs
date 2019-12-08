using System;
using System.Collections;
using System.Threading.Tasks;
using Windows.UI;
using GroupNStegafy.Constants;
using GroupNStegafy.Converter;
using GroupNStegafy.Enumerables;
using GroupNStegafy.Formatter;
using GroupNStegafy.Utility;
using GroupNStegafy.View;

namespace GroupNStegafy.Model.Embedding
{
    /// <summary>
    ///     Stores information for the TextMessageEmbedder class
    /// </summary>
    /// <seealso cref="MessageEmbedder" />
    public class TextMessageEmbedder : MessageEmbedder
    {
        #region Data members

        private int currentByteIndex;
        private int numberOfBitsInByte = 8;

        #endregion

        #region Methods

        /// <summary>
        ///     Embeds the text message in image.
        /// </summary>
        /// @Precondition none
        /// @Postcondition the message is embedded into the image
        /// <param name="messageData">The message data.</param>
        /// <param name="messageLength">Length of the message.</param>
        /// <param name="messageImageHeight">Height of the message image.</param>
        /// <param name="sourceImageWidth">Width of the source image.</param>
        /// <param name="sourceImageHeight">Height of the source image.</param>
        /// <param name="encryptionIsChecked">if set to <c>true</c> [encryption is selected].</param>
        /// <param name="bpcc">The BPCC.</param>
        /// <returns>Task</returns>
        /// <exception cref="NotImplementedException"></exception>
        public override async Task EmbedMessageInImage(byte[] messageData, uint messageLength, uint messageImageHeight,
            uint sourceImageWidth, uint sourceImageHeight, bool encryptionIsChecked, int bpcc)
        {
            var totalAvailableSourcePixels = sourceImageWidth * sourceImageHeight - 2;
            this.currentByteIndex = 0;
            var numberOfBits = messageData.Length * this.numberOfBitsInByte;

            if (numberOfBits / bpcc > totalAvailableSourcePixels * PixelConstants.NumberOfColorChannels)
            {
                var requiredBpcc = this.calculateBpccRequiredToEmbedText(numberOfBits, totalAvailableSourcePixels);
                if (requiredBpcc > this.numberOfBitsInByte)
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
                        sourcePixelColor = this.embedTextMessage(messageData, sourcePixelColor, currentIndex, bpcc);
                        currentIndex += PixelConstants.NumberOfColorChannels * bpcc;
                    }

                    PixelColorInfo.SetPixelBgra8(SourceImagePixels, currY, currX, sourcePixelColor, sourceImageWidth);
                }
            }

            await SetEmbeddedImage(sourceImageHeight, sourceImageWidth);
        }

        private Color embedTextMessage(byte[] messageData, Color sourcePixelColor, int currentIndex, int bpcc)
        {
            var pixelInfo = ColorByteArrayConverter.GetByteArray(sourcePixelColor);
            var count = 0;
            var embeddedPixelInfo = new byte[PixelConstants.NumberOfColorChannels];

            foreach (var colorInfo in pixelInfo)
            {
                var currColor = colorInfo;
                currColor >>= bpcc;
                currColor <<= bpcc;

                var bitsToAdd = new BitArray(this.numberOfBitsInByte);
                for (var j = 0; j < bpcc; j++)
                {
                    if (currentIndex + j < messageData.Length * this.numberOfBitsInByte)
                    {
                        var currentByte = messageData[this.currentByteIndex];
                        var currentBit = isBitSet(currentByte, currentIndex % 8);
                        bitsToAdd.Set(j, currentBit);

                        if (currentIndex != 0 && (currentIndex + j) % this.numberOfBitsInByte == 0)
                        {
                            this.currentByteIndex++;
                        }
                    }
                    else
                    {
                        bitsToAdd.Set(j, false);
                    }
                }

                var bitsAsByte = new byte[1];
                bitsToAdd.CopyTo(bitsAsByte, 0);

                currColor |= bitsAsByte[0];
                embeddedPixelInfo[count] = currColor;

                currentIndex += bpcc;
                count++;
            }

            return Color.FromArgb(0, embeddedPixelInfo[0], embeddedPixelInfo[1], embeddedPixelInfo[2]);
        }

        private int calculateBpccRequiredToEmbedText(int bitCount, uint totalSourcePixels)
        {
            var requiredBpcc = 1;

            for (var i = 1; bitCount / i > totalSourcePixels * PixelConstants.NumberOfColorChannels; i++)
            {
                requiredBpcc++;
            }

            return requiredBpcc;
        }

        private static bool isBitSet(byte bits, int pos)
        {
            return (bits & (1 << pos)) != 0;
        }

        #endregion
    }
}