using Windows.UI;
using GroupNStegafy.Constants;

namespace GroupNStegafy.Converter
{
    /// <summary>
    ///     Stores information for the ColorToByteArray class
    /// </summary>
    public static class ColorByteArrayConverter
    {
        #region Methods

        /// <summary>
        ///     Gets the byte array.
        /// </summary>
        /// @Precondition none
        /// @Postcondition none
        /// <param name="color">The color.</param>
        /// <returns>The RGB byte array of the color</returns>
        public static byte[] GetByteArray(Color color)
        {
            var byteArray = new byte[PixelConstants.NumberOfColorChannels];
            byteArray[0] = color.R;
            byteArray[1] = color.G;
            byteArray[2] = color.B;

            return byteArray;
        }

        #endregion
    }
}