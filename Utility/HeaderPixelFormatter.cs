using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using GroupNStegafy.Converter;

namespace GroupNStegafy.Utility
{
    /// <summary>Stores methods for formatting the header pixels</summary>
    public static class HeaderPixelFormatter
    {
        private const int FirstPixelAmount = 212;

        public  static Color FormatFirstHeaderPixel(Color sourcePixelColor)
        {
            sourcePixelColor.R = FirstPixelAmount;
            sourcePixelColor.G = FirstPixelAmount;
            sourcePixelColor.B = FirstPixelAmount;

            return sourcePixelColor;
        }

        public static Color FormatSecondHeaderPixel(StorageFile messageFile, Color sourcePixelColor, bool isChecked, int bpcc)
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
            var binaryBpcc = BinaryDecimalConverter.CalculateBinaryForBpcc(bpcc); // convert bpcc to binary representation
            sourcePixelColor.G = (byte)binaryBpcc; // set the green channel to binary representation of bpcc selection

            return sourcePixelColor;
        }

        private static Color handleEncryptionSelectionHeader(bool? isChecked, Color sourcePixelColor)
        {
            if (isChecked != null && (bool)isChecked)
            {
                sourcePixelColor.R |= 1; //set LSB red source pixel to 1
            }
            else
            {
                sourcePixelColor.R &= 0xfe; //set LSB red source pixel to 0
            }
            return sourcePixelColor;
        }
    }
}
