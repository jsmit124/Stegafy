using System.Text;

namespace GroupNStegafy.Converter
{
    /// <summary>
    ///     Stores methods for converting between binary and strings
    /// </summary>
    public static class BinaryStringConverter
    {
        #region Methods

        /// <summary>
        ///     Converts the string to binary.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>The binary representation of the string</returns>
        public static byte[] ConvertStringToBinary(string text)
        {
            return Encoding.ASCII.GetBytes(text);
        }

        /// <summary>
        ///     Converts the binary to string.
        /// </summary>
        /// <param name="stringBytes">The string bytes.</param>
        /// <returns>The string representation of the binary array</returns>
        public static string ConvertBinaryToString(byte[] stringBytes)
        {
            return Encoding.ASCII.GetString(stringBytes);
        }

        #endregion
    }
}