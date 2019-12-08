using System.Collections;
using System.Text;

namespace GroupNStegafy.Converter
{
    /// <summary>
    ///     Stores methods for converting between binary and strings
    /// </summary>
    public static class BinaryStringConverter
    {
        #region Data members

        private static readonly int numberOfBitsInByte = 8;

        #endregion

        #region Methods

        /// <summary>
        ///     Converts the string to binary.
        /// </summary>
        /// @Precondition none
        /// @Postcondition none
        /// <param name="text">The text.</param>
        /// <returns>The binary representation of the string</returns>
        public static byte[] ConvertStringToBinary(string text)
        {
            return Encoding.ASCII.GetBytes(text);
        }

        /// <summary>
        ///     Bits the array to string.
        /// </summary>
        /// @Precondition none
        /// @Postcondition none
        /// <param name="bits">The bits.</param>
        /// <returns>The string representation of the bit array</returns>
        public static string BitArrayToString(BitArray bits)
        {
            var stringArray = new byte[bits.Length / numberOfBitsInByte];

            var encoding = new ASCIIEncoding();

            for (var i = 0; i < bits.Length / numberOfBitsInByte; i++)
            {
                for (int index = i * numberOfBitsInByte, j = 1;
                    index < i * numberOfBitsInByte + numberOfBitsInByte;
                    index++, j *= 2)
                {
                    stringArray[i] += bits.Get(index) ? (byte) j : (byte) 0;
                }
            }

            return encoding.GetString(stringArray);
        }

        #endregion
    }
}