using Windows.Storage;
using Windows.UI;
using GroupNStegafy.Converter;

namespace GroupNStegafy.Utility
{
    /// <summary>
    ///     Stores methods for formatting the header pixels
    /// </summary>
    public static class HeaderPixelFormatter
    {
        #region Data members

        private const int FirstPixelAmount = 212;

        #endregion

        #region Methods

        /// <summary>
        ///     Formats the first header pixel.
        /// </summary>
        /// <param name="sourcePixelColor">Color of the source pixel.</param>
        /// <returns>The pixel color of the first header pixel</returns>
        public static Color FormatFirstHeaderPixel(Color sourcePixelColor)
        {
            sourcePixelColor.R = FirstPixelAmount;
            sourcePixelColor.G = FirstPixelAmount;
            sourcePixelColor.B = FirstPixelAmount;

            return sourcePixelColor;
        }

        /// <summary>
        ///     Formats the second header pixel.
        /// </summary>
        /// <param name="messageFile">The message file.</param>
        /// <param name="sourcePixelColor">Color of the source pixel.</param>
        /// <param name="isChecked">if set to <c>true</c> [is checked].</param>
        /// <param name="bpcc">The BPCC.</param>
        /// <returns>The pixel color of the second header pixel</returns>
        public static Color FormatSecondHeaderPixel(StorageFile messageFile, Color sourcePixelColor, bool isChecked,
            int bpcc)
        {
            sourcePixelColor = handleEncryptionSelectionHeader(isChecked, sourcePixelColor);
            sourcePixelColor = handleBpccSelectionHeader(bpcc, sourcePixelColor);
            sourcePixelColor = handleEmbeddingTypeHeader(sourcePixelColor, messageFile);

            return sourcePixelColor;
        }

        private static Color handleEmbeddingTypeHeader(Color sourcePixelColor, StorageFile file)
        {
            if (file.FileType.Equals(".txt"))
            {
                sourcePixelColor.B |= 1; //set LSB blue source pixel to 1
            }
            else
            {
                sourcePixelColor.B &= 0xfe; //set LSB blue source pixel to 0
            }

            return sourcePixelColor;
        }

        private static Color handleBpccSelectionHeader(int bpcc, Color sourcePixelColor)
        {
            var binaryBpcc =
                BinaryDecimalConverter.CalculateBinaryForBpcc(bpcc); // convert bpcc to binary representation
            sourcePixelColor.G = (byte) binaryBpcc; // set the green channel to binary representation of bpcc selection

            return sourcePixelColor;
        }

        private static Color handleEncryptionSelectionHeader(bool? isChecked, Color sourcePixelColor)
        {
            if (isChecked != null && (bool) isChecked)
            {
                sourcePixelColor.R |= 1; //set LSB red source pixel to 1
            }
            else
            {
                sourcePixelColor.R &= 0xfe; //set LSB red source pixel to 0
            }

            return sourcePixelColor;
        }

        #endregion
    }
}