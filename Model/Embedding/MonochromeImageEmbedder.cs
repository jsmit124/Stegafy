using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI;
using GroupNStegafy.Enumerables;
using GroupNStegafy.Formatter;
using GroupNStegafy.Utility;
using GroupNStegafy.View;

namespace GroupNStegafy.Model.Embedding
{
    /// <summary>
    ///     Stores information for the MonochromeImageEmbedder class
    /// </summary>
    /// <seealso cref="MessageEmbedder" />
    public class MonochromeImageEmbedder : MessageEmbedder
    {
        #region Methods

        /// <summary>
        ///     Embeds the message in image.
        /// </summary>
        /// <param name="messagePixels">The message pixels.</param>
        /// <param name="messageImageWidth">Width of the message image.</param>
        /// <param name="messageImageHeight">Height of the message image.</param>
        /// <param name="sourceImageWidth">Width of the source image.</param>
        /// <param name="sourceImageHeight">Height of the source image.</param>
        /// <param name="encryptionIsChecked">if set to <c>true</c> [encryption is checked].</param>
        /// <param name="bpcc">The BPCC.</param>
        public override async Task EmbedMessageInImage(byte[] messagePixels, uint messageImageWidth,
            uint messageImageHeight, uint sourceImageWidth, uint sourceImageHeight, bool encryptionIsChecked, int bpcc)
        {
            if (messageImageWidth > sourceImageWidth || messageImageHeight > sourceImageHeight)
            {
                await Dialogs.ShowMessageFileTooLargeDialog();
                MessageTooLarge = true;
                return;
            }


            if (encryptionIsChecked)
            {
                messagePixels = swapMessagePixelQuadrants(messagePixels);
            }

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
                        sourcePixelColor = HeaderPixelFormatter.FormatSecondHeaderPixel(FileTypes.Bitmap,
                            sourcePixelColor, encryptionIsChecked, bpcc);
                    }
                    else
                    {
                        sourcePixelColor = this.embedMonochromeImage(currX, messageImageWidth, currY,
                            messageImageHeight, messagePixels, sourcePixelColor);
                    }

                    PixelColorInfo.SetPixelBgra8(SourceImagePixels, currY, currX, sourcePixelColor, sourceImageWidth);
                }
            }

            await SetEmbeddedImage(sourceImageHeight, sourceImageWidth);
        }

        private static byte[] swapMessagePixelQuadrants(byte[] messagePixels)
        {
            var splitMessageLength = messagePixels.Length / 2;
            var topLeft = new byte[splitMessageLength];
            Array.Copy(messagePixels, 0, topLeft, 0, splitMessageLength);
            var topRight = new byte[splitMessageLength];
            Array.Copy(messagePixels, splitMessageLength, topRight, 0, splitMessageLength);

            var swappedArrays = new List<byte>();
            swappedArrays.AddRange(topRight);
            swappedArrays.AddRange(topLeft);

            var swappedBytes = swappedArrays.ToArray();

            messagePixels = swappedBytes;
            return messagePixels;
        }

        private Color embedMonochromeImage(int currX, uint messageImageWidth, int currY, uint messageImageHeight,
            byte[] messagePixels, Color sourcePixelColor)
        {
            if (currX < messageImageWidth && currY < messageImageHeight)
            {
                var messagePixelColor = PixelColorInfo.GetPixelBgra8(messagePixels, currY,
                    currX, messageImageWidth);

                if (isBlackPixel(messagePixelColor))
                {
                    sourcePixelColor.B &= 0xfe; //set LSB blue source pixel to 0
                }
                else if (isWhitePixel(messagePixelColor))
                {
                    sourcePixelColor.B |= 1; //set LSB blue source pixel to 1
                }
            }

            return sourcePixelColor;
        }

        private static bool isWhitePixel(Color messagePixelColor)
        {
            return messagePixelColor.R == 255
                   && messagePixelColor.B == 255
                   && messagePixelColor.G == 255;
        }

        private static bool isBlackPixel(Color messagePixelColor)
        {
            return messagePixelColor.R == 0
                   && messagePixelColor.B == 0
                   && messagePixelColor.G == 0;
        }

        #endregion
    }
}