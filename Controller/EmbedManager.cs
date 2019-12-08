using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;
using GroupNStegafy.Constants;
using GroupNStegafy.Converter;
using GroupNStegafy.Formatter;
using GroupNStegafy.IO;
using GroupNStegafy.Model.Embedding;
using GroupNStegafy.Utility;

namespace GroupNStegafy.Controller
{
    /// <summary>
    ///     Stores information for the StegafyManager class
    /// </summary>
    public class EmbedManager
    {
        #region Data members

        private readonly FileWriter fileWriter;
        private readonly FileReader fileReader;
        private MessageEmbedder messageEmbedder;
        private StorageFile sourceImageFile;
        private StorageFile messageFile;
        private double dpiX;
        private double dpiY;
        private uint sourceImageWidth;
        private uint sourceImageHeight;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the embedded image.
        /// </summary>
        /// <value>The embedded image.</value>
        public WriteableBitmap EmbeddedImage => this.messageEmbedder.EmbeddedImage;

        /// <summary>
        ///     Gets a value indicating whether [message too large].
        /// </summary>
        /// <value><c>true</c> if [message too large]; otherwise, <c>false</c>.</value>
        public bool MessageTooLarge => this.messageEmbedder.MessageTooLarge;

        /// <summary>
        ///     Gets the source image.
        /// </summary>
        /// <value>The source image.</value>
        public BitmapImage SourceImage { get; private set; }

        /// <summary>
        ///     Gets the message image.
        /// </summary>
        /// <value>The message image.</value>
        public BitmapImage MessageImage { get; private set; }

        /// <summary>
        ///     Gets the type of the message file.
        /// </summary>
        /// <value>The type of the message file.</value>
        public string MessageFileType => this.messageFile.FileType;

        /// <summary>
        ///     Gets the text from file.
        /// </summary>
        /// <value>The text from file.</value>
        public string TextFromFile { get; private set; }

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="EmbedManager" /> class.
        /// </summary>
        public EmbedManager()
        {
            this.fileReader = new FileReader();
            this.fileWriter = new FileWriter();

            this.messageEmbedder = null;
            this.messageFile = null;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Saves the embedded image.
        ///     @Precondition none
        ///     @Postcondition embedded image is saved
        /// </summary>
        public void SaveEmbeddedImage()
        {
            this.fileWriter.SaveWritableBitmap(this.EmbeddedImage, this.dpiX, this.dpiY);
        }

        /// <summary>
        ///     Loads the message.
        ///     @Precondition none
        ///     @Postcondition message file is loaded into the program
        /// </summary>
        public async Task LoadMessage()
        {
            this.messageFile = await this.fileReader.SelectMessageFile();
            if (this.messageFile == null)
            {
                return;
            }

            if (this.MessageFileType.Equals(FileTypeConstants.TextFileType))
            {
                this.messageEmbedder = new TextMessageEmbedder();
                this.TextFromFile = await this.fileReader.ReadTextFromFile(this.messageFile);
            }
            else
            {
                this.messageEmbedder = new MonochromeImageEmbedder();
                this.MessageImage = await FileBitmapConverter.ConvertFileToBitmap(this.messageFile);
            }

            if (this.SourceImage != null)
            {
                var sourceImagePixels = await PixelExtracter.ExtractPixelDataFromFile(this.sourceImageFile);
                this.messageEmbedder.SourceImagePixels = sourceImagePixels;
            }
        }

        /// <summary>
        ///     Loads the source image.
        ///     @Precondition none
        ///     @Postcondition source image is loaded into the program
        /// </summary>
        public async Task LoadSourceImage()
        {
            this.sourceImageFile = await this.fileReader.SelectSourceImageFile();
            if (this.sourceImageFile == null)
            {
                return;
            }

            var sourceImage = await FileBitmapConverter.ConvertFileToBitmap(this.sourceImageFile);
            this.SourceImage = sourceImage;

            var sourceImagePixels = await PixelExtracter.ExtractPixelDataFromFile(this.sourceImageFile);
            this.messageEmbedder.SourceImagePixels = sourceImagePixels;

            await this.setSourceImageSizeValues();
        }

        /// <summary>
        ///     Embeds the message.
        /// </summary>
        /// @Precondition this.messageFile != null && this.sourceFile != null
        /// @Postcondition message is embedded and shown in the program
        /// <param name="encryptionSelected">if set to <c>true</c> [encryption selected].</param>
        /// <param name="bpcc">The BPCC.</param>
        /// <param name="encryptionKey">The encryption key.</param>
        public async Task EmbedMessage(bool encryptionSelected, int bpcc, string encryptionKey)
        {
            if (this.messageFile.FileType == FileTypeConstants.TextFileType)
            {
                string formattedText;
                if (encryptionSelected)
                {
                    formattedText =
                        EmbedTextFormatter.FormatEncryptedTextForEmbedding(encryptionKey, this.TextFromFile);
                }
                else
                {
                    formattedText = EmbedTextFormatter.FormatTextForEmbedding(this.TextFromFile);
                }

                var binaryText = BinaryStringConverter.ConvertStringToBinary(formattedText);
                var messageLength = binaryText.Length;

                await this.messageEmbedder.EmbedMessageInImage(binaryText, (uint) messageLength, 0,
                    this.sourceImageWidth, this.sourceImageHeight, encryptionSelected, bpcc);
            }
            else
            {
                var messageDecoder =
                    await BitmapDecoder.CreateAsync(await this.messageFile.OpenAsync(FileAccessMode.Read));
                var messagePixels = await PixelExtracter.ExtractPixelDataFromFile(this.messageFile);
                var messageImageWidth = messageDecoder.PixelWidth;
                var messageImageHeight = messageDecoder.PixelHeight;

                await this.messageEmbedder.EmbedMessageInImage(messagePixels, messageImageWidth, messageImageHeight,
                    this.sourceImageWidth, this.sourceImageHeight, encryptionSelected, bpcc);
            }
        }

        private int getImageHalfHeight(BitmapImage image)
        {
            if (this.imageHeightIsEven(image))
            {
                return image.PixelHeight / 2;
            }

            return (image.PixelHeight - 1) / 2;
        }

        private bool imageHeightIsEven(BitmapImage image)
        {
            var heightIsEven = image.PixelHeight % 2 == 0;
            return heightIsEven;
        }

        /// <summary>
        ///     Checks to see if the messageFile is loaded
        /// </summary>
        /// @Precondition none
        /// @Postcondition none
        /// <returns>true if the messageFile != null; false otherwise</returns>
        public bool MessageLoaded()
        {
            return this.messageFile != null;
        }

        /// <summary>
        ///     Checks to see if the sourceImageFile is loaded
        /// </summary>
        /// @Precondition none
        /// @Postcondition none
        /// <returns>true if the sourceImageFile is loaded; false otherwise</returns>
        public bool SourceImageLoaded()
        {
            return this.sourceImageFile != null;
        }

        private async Task setSourceImageSizeValues()
        {
            using (var fileStream = await this.sourceImageFile.OpenAsync(FileAccessMode.Read))
            {
                var decoder = await BitmapDecoder.CreateAsync(fileStream);
                this.dpiX = decoder.DpiX;
                this.dpiY = decoder.DpiY;
                this.sourceImageHeight = decoder.PixelHeight;
                this.sourceImageWidth = decoder.PixelWidth;
            }
        }

        #endregion
    }
}