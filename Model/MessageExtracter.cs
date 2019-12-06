
using System.Threading.Tasks;

namespace GroupNStegafy.Model
{
    public abstract class MessageExtracter
    {
        public abstract Task ExtractMessageFromImage(byte[] embeddedPixels, uint embeddedImageWidth, uint embeddedImageHeight);

        protected static bool isSecondPixel(int currY, int currX)
        {
            return currY == 0 && currX == 1;
        }

        protected static bool isFirstPixel(int currY, int currX)
        {
            return currY == 0 && currX == 0;
        }
    }
}
