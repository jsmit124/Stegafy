using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace GroupNStegafy.Model.Extracting
{
    public abstract class MessageExtracter
    {
        public byte[] EmbeddedPixels;
        public WriteableBitmap ExtractedMessage { get; protected set; }

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
