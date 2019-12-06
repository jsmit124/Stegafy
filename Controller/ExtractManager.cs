
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;
using GroupNStegafy.Converter;
using GroupNStegafy.IO;
using GroupNStegafy.Model;

namespace GroupNStegafy.Controller
{
    public class ExtractManager
    {
        private readonly FileWriter fileWriter;
        private readonly FileReader fileReader;
        private double dpiX;
        private double dpiY;
        private MessageExtracter messageExtracter;

        public StorageFile EmbeddedImageFile { get; private set; }

        public BitmapImage EmbeddedImage { get; private set; }

        public ExtractManager()
        {
            this.fileReader = new FileReader();
            this.fileWriter = new FileWriter();

            this.EmbeddedImageFile = null;
            this.messageExtracter = null;
        }

        public void SaveExtractedMessage(WriteableBitmap extractedImage, double dpiX, double dpiY)
        {
            this.fileWriter.SaveWritableBitmap(extractedImage, dpiX, dpiY);
        }

        //public void ExtractMessage()
        //{
        //    this.messageExtracter.ExtractMessageFromImage(//TODO);
        //}

        public async Task LoadEmbeddedImage()
        {
            this.EmbeddedImageFile = await this.fileReader.SelectSourceImageFile();
            if (this.EmbeddedImageFile == null)
            {
                return;
            }

            this.EmbeddedImage = await FileBitmapConverter.ConvertFileToBitmap(this.EmbeddedImageFile);
            
        }
    }
}
