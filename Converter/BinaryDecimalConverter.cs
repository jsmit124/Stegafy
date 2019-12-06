using System;

namespace GroupNStegafy.Converter
{
    /// <summary>
    ///     Stores methods for converting between binary and decimal numbers
    /// </summary>
    public static class BinaryDecimalConverter
    {
        #region Methods

        /// <summary>
        ///     Calculates the binary for BPCC.
        /// </summary>
        /// <param name="bpccSelection">The BPCC selection.</param>
        /// <returns>The integer (decimal) representation of the necessary binary number to be stored as BPCC</returns>
        public static int CalculateBinaryForBpcc(int bpccSelection)
        {
            var sum = 0.0;
            for (var i = 0; i < bpccSelection; i++)
            {
                sum += Math.Pow(2, i);
            }

            return Convert.ToInt32(sum);
        }

        /// <summary>
        ///     Calculates the BPCC from binary.
        /// </summary>
        /// <param name="bits">The bits.</param>
        /// <returns>The Bpcc found in the byte</returns>
        public static int CalculateBpccFromBinary(byte bits)
        {
            var bpcc = 1;
            for (var i = 1; CalculateBinaryForBpcc(i) < bits; i++)
            {
                bpcc++;
            }

            return bpcc;
        }

        #endregion
    }
}