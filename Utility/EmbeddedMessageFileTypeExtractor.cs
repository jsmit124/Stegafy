using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GroupNStegafy.Enumerables;

namespace GroupNStegafy.Utility
{
    public static class EmbeddedMessageFileTypeExtractor
    {
        public static FileTypes DetermineFileTypeToExtract(byte[] pixelInformation, uint imageWidth)
        {
            var pixelColor = PixelColorInfo.GetPixelBgra8(pixelInformation, 1, 0, imageWidth);
            var lastBlueBit = pixelColor.B << 7;

            if ((pixelColor.B & 1) != 0)
            {
                return FileTypes.Text;
            }
            else
            {
                return FileTypes.Bitmap;
            }
        }
    }
}
