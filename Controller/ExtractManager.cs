
using System;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;
using GroupNStegafy.Converter;
using GroupNStegafy.Enumerables;
using GroupNStegafy.IO;
using GroupNStegafy.Model;
using GroupNStegafy.Model.Extracting;
using GroupNStegafy.Utility;

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
        public WriteableBitmap ExtractedImage => this.messageExtracter.ExtractedImage;
        public WriteableBitmap DecryptedImage => this.messageExtracter.DecryptedImage;
        public string ExtractedText => this.messageExtracter.ExtractedText;
        public string DecryptedText => this.messageExtracter.DecryptedText;
        public bool EncryptionUsed => this.messageExtracter.EncryptionUsed;


        public ExtractManager()
        {
            this.fileReader = new FileReader();
            this.fileWriter = new FileWriter();

            this.EmbeddedImageFile = null;
            this.messageExtracter = null;

            this.dpiX = 0;
            this.dpiY = 0;
        }

        public void SaveExtractedMessage()
        {
            this.fileWriter.SaveWritableBitmap(this.ExtractedImage, this.dpiX, this.dpiY);
        }

        public async Task ExtractMessage()
        {
            var embeddedPixels = await PixelExtracter.ExtractPixelDataFromFile(this.EmbeddedImageFile);
            var embeddedDecoder =
                await BitmapDecoder.CreateAsync(await this.EmbeddedImageFile.OpenAsync(FileAccessMode.Read));

            var fileTypeEmbedded = EmbeddedMessageFileTypeExtractor.DetermineFileTypeToExtract(embeddedPixels, embeddedDecoder.PixelWidth);

            if (fileTypeEmbedded == FileTypes.Text)
            {
                this.messageExtracter = new TextFileExtracter();
            }
            else
            {
                this.messageExtracter = new MonochromeImageExtracter();
            }

            await this.setExtractedImageSizeValues();

            await this.messageExtracter.ExtractMessageFromImage(embeddedPixels, embeddedDecoder.PixelWidth, embeddedDecoder.PixelHeight);
        }

        public async Task LoadEmbeddedImage()
        {
            this.EmbeddedImageFile = await this.fileReader.SelectSourceImageFile();
            if (this.EmbeddedImageFile == null)
            {
                return;
            }

            this.EmbeddedImage = await FileBitmapConverter.ConvertFileToBitmap(this.EmbeddedImageFile);
        }

        private async Task setExtractedImageSizeValues()
        {
            using (var fileStream = await this.EmbeddedImageFile.OpenAsync(FileAccessMode.Read))
            {
                var decoder = await BitmapDecoder.CreateAsync(fileStream);
                this.dpiX = decoder.DpiX;
                this.dpiY = decoder.DpiY;
            }
        }
    }
}
