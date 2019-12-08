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
            var stringArray = new byte[bits.Length / 8];

            var encoding = new ASCIIEncoding();

            for (var i = 0; i < bits.Length / 8; i++)
            {
                for (int index = i * 8, j = 1; index < i * 8 + 8; index++, j *= 2)
                {
                    stringArray[i] += bits.Get(index) ? (byte) j : (byte) 0;
                }
            }

            return encoding.GetString(stringArray);
        }

        /// <summary>
        ///     Reverses the byte.
        /// </summary>
        /// <param name="input">The input.</param>
        /// @Precondition none
        /// @Postcondition none
        /// <returns>the reversed byte</returns>
        public static BitArray ReverseByte(BitArray input)
        {
            var bar = new BitArray(8);

            for (var i = 0; i < 8; i++)
            {
                bar[i] = input[8 - i - 1];
            }

            return bar;
        }

        #endregion
    }
}