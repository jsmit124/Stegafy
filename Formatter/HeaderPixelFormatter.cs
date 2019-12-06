using Windows.UI;
using GroupNStegafy.Converter;
using GroupNStegafy.Enumerables;

namespace GroupNStegafy.Formatter
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
        /// <param name="fileType">The file type.</param>
        /// <param name="sourcePixelColor">Color of the source pixel.</param>
        /// <param name="encryptionUsed">if set to <c>true</c> [is checked].</param>
        /// <param name="bpcc">The BPCC.</param>
        /// <returns>The pixel color of the second header pixel</returns>
        public static Color FormatSecondHeaderPixel(FileTypes fileType, Color sourcePixelColor, bool encryptionUsed,
            int bpcc)
        {
            sourcePixelColor = handleEncryptionSelectionHeader(encryptionUsed, sourcePixelColor);
            sourcePixelColor = handleBpccSelectionHeader(bpcc, sourcePixelColor);
            sourcePixelColor = handleEmbeddingTypeHeader(sourcePixelColor, fileType);

            return sourcePixelColor;
        }

        private static Color handleEmbeddingTypeHeader(Color sourcePixelColor, FileTypes fileType)
        {
            if (fileType == FileTypes.Text)
            {
                sourcePixelColor.B |= 1;
            }
            else
            {
                sourcePixelColor.B &= 0xfe;
            }

            return sourcePixelColor;
        }

        private static Color handleBpccSelectionHeader(int bpcc, Color sourcePixelColor)
        {
            var binaryBpcc =
                BinaryDecimalConverter.CalculateBinaryForBpcc(bpcc);
            sourcePixelColor.G = (byte) binaryBpcc;

            return sourcePixelColor;
        }

        private static Color handleEncryptionSelectionHeader(bool? encryptionUsed, Color sourcePixelColor)
        {
            if (encryptionUsed != null && (bool) encryptionUsed)
            {
                sourcePixelColor.R |= 1;
            }
            else
            {
                sourcePixelColor.R &= 0xfe;
            }

            return sourcePixelColor;
        }

        #endregion
    }
}