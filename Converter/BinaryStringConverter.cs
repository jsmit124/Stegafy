using System;
using System.Collections;
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

        public static string BitArrayToStr(BitArray ba)
        {
            byte[] strArr = new byte[ba.Length / 8];

            ASCIIEncoding encoding = new System.Text.ASCIIEncoding();

            for (int i = 0; i < ba.Length / 8; i++)
            {
                for (int index = i * 8, m = 1; index < i * 8 + 8; index++, m *= 2)
                {
                    strArr[i] += ba.Get(index) ? (byte)m : (byte)0;
                }
            }

            return encoding.GetString(strArr);
        }

        #endregion
    }
}