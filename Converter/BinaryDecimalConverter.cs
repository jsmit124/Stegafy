using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GroupNStegafy.Converter
{
    public static class BinaryDecimalConverter
    {
        public static int CalculateBinaryForBpcc(int bpccSelection)
        {
            var sum = 0.0;
            for (var i = 0; i < bpccSelection; i++)
            {
                sum += Math.Pow(2, i);
            }
            return Convert.ToInt32(sum);
        }
    }
}
