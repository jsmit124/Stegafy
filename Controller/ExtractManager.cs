using System;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;
using GroupNStegafy.Converter;
using GroupNStegafy.Enumerables;
using GroupNStegafy.IO;
using GroupNStegafy.Model.Extracting;
using GroupNStegafy.Utility;

namespace GroupNStegafy.Controller
{
    /// <summary>
    ///     Stores information for the ExtractManager class
    /// </summary>
    public class ExtractManager
    {
        #region Data members

        private readonly FileWriter fileWriter;
        private readonly FileReader fileReader;
        private double dpiX;
        private double dpiY;
        private MessageExtracter messageExtracter;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the embedded image file.
        /// </summary>
        /// <value>The embedded image file.</value>
        public StorageFile EmbeddedImageFile { get; private set; }

        /// <summary>
        ///     Gets the embedded image.
        /// </summary>
        /// <value>The embedded image.</value>
        public BitmapImage EmbeddedImage { get; private set; }

        /// <summary>
        ///     Gets the extracted image.
        /// </summary>
        /// <value>The extracted image.</value>
        public WriteableBitmap ExtractedImage => this.messageExtracter.ExtractedImage;

        /// <summary>
        ///     Gets the decrypted image.
        /// </summary>
        /// <value>The decrypted image.</value>
        public WriteableBitmap DecryptedImage => this.messageExtracter.DecryptedImage;

        /// <summary>
        ///     Gets the extracted text.
        /// </summary>
        /// <value>The extracted text.</value>
        public string ExtractedText => this.messageExtracter.ExtractedText;

        /// <summary>
        ///     Gets the decrypted text.
        /// </summary>
        /// <value>The decrypted text.</value>
        public string DecryptedText => this.messageExtracter.DecryptedText;

        /// <summary>
        ///     Gets a value indicating whether [encryption used].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [encryption used]; otherwise, <c>false</c>.
        /// </value>
        public bool EncryptionUsed => this.messageExtracter.EncryptionUsed;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExtractManager" /> class.
        /// </summary>
        public ExtractManager()
        {
            this.fileReader = new FileReader();
            this.fileWriter = new FileWriter();

            this.EmbeddedImageFile = null;
            this.messageExtracter = null;

            this.dpiX = 0;
            this.dpiY = 0;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Saves the extracted message.
        /// </summary>
        /// @Precondition none
        /// @Postcondition saves the extracted message
        public void SaveExtractedMessage()
        {
            this.fileWriter.SaveWritableBitmap(this.ExtractedImage, this.dpiX, this.dpiY);
        }

        /// <summary>
        ///     Extracts the message.
        /// </summary>
        /// @Precondition this.embeddedImageFile != null
        /// @Postcondition message is extracted from image
        public async Task ExtractMessage()
        {
            var embeddedPixels = await PixelExtracter.ExtractPixelDataFromFile(this.EmbeddedImageFile);
            var embeddedDecoder =
                await BitmapDecoder.CreateAsync(await this.EmbeddedImageFile.OpenAsync(FileAccessMode.Read));

            var fileTypeEmbedded =
                EmbeddedMessageFileTypeExtractor.DetermineFileTypeToExtract(embeddedPixels, embeddedDecoder.PixelWidth);

            if (fileTypeEmbedded == FileTypes.Text)
            {
                this.messageExtracter = new TextFileExtracter();
            }
            else
            {
                this.messageExtracter = new MonochromeImageExtracter();
            }

            await this.setExtractedImageSizeValues();

            await this.messageExtracter.ExtractMessageFromImage(embeddedPixels, embeddedDecoder.PixelWidth,
                embeddedDecoder.PixelHeight);
        }

        /// <summary>
        ///     Loads the embedded image
        /// </summary>
        /// @Precondition none
        /// @Postcondition embedded image is loaded
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

        #endregion
    }
}