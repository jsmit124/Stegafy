using System.Collections;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using GroupNStegafy.Constants;
using GroupNStegafy.Converter;
using GroupNStegafy.Cryptography;
using GroupNStegafy.Utility;
using GroupNStegafy.View;

namespace GroupNStegafy.Model.Extracting
{
    /// <summary>
    ///     Stores information for the TextFileExtractor class
    /// </summary>
    /// <seealso cref="GroupNStegafy.Model.Extracting.MessageExtracter" />
    public class TextFileExtracter : MessageExtracter
    {
        #region Data members

        private int bpcc;
        private BitArray embeddedBits;
        private readonly StringBuilder peek;
        private readonly StringBuilder embeddedMessage;
        private readonly StringBuilder encryptionKey;
        private bool endOfMessageReached;
        private readonly int numberOfBitsInByte = 8;
        private bool endOfEncryptionKeyReached;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="TextFileExtracter" /> class.
        /// </summary>
        public TextFileExtracter()
        {
            this.peek = new StringBuilder(this.numberOfBitsInByte);
            this.embeddedMessage = new StringBuilder(this.numberOfBitsInByte);
            this.encryptionKey = new StringBuilder();
            this.endOfMessageReached = false;
            this.endOfEncryptionKeyReached = false;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Extracts the message from image.
        /// </summary>
        /// <param name="embeddedPixels">The embedded pixels.</param>
        /// <param name="embeddedImageWidth">Width of the embedded image.</param>
        /// <param name="embeddedImageHeight">Height of the embedded image.</param>
        /// @Precondition none
        /// @Postcondition message is extracted from the image
        public override async Task ExtractMessageFromImage(byte[] embeddedPixels, uint embeddedImageWidth,
            uint embeddedImageHeight)
        {
            var encryptionPassword = new StringBuilder();
            var count = 0;

            for (var currY = 0; currY < embeddedImageHeight; currY++)
            {
                for (var currX = 0; currX < embeddedImageWidth; currX++)
                {
                    var embeddedPixelColor = PixelColorInfo.GetPixelBgra8(embeddedPixels, currY, currX,
                        embeddedImageWidth);

                    if (isFirstPixel(currY, currX))
                    {
                        if (embeddedPixelColor.R == 212 && embeddedPixelColor.B == 212 &&
                            embeddedPixelColor.G == 212)
                        {
                            continue;
                        }

                        await Dialogs.ShowNoMessageDialog();
                        return;
                    }

                    if (isSecondPixel(currY, currX))
                    {
                        EncryptionUsed = (embeddedPixelColor.R & 1) != 0;
                        this.endOfEncryptionKeyReached = !EncryptionUsed;
                        this.bpcc = BinaryDecimalConverter.CalculateBpccFromBinary(embeddedPixelColor.G);

                        var numberOfBits = (embeddedImageWidth * embeddedImageHeight - 2) *
                                           PixelConstants.NumberOfColorChannels * this.bpcc;
                        this.embeddedBits = new BitArray((int) numberOfBits);
                    }
                    else
                    {
                        if (this.endOfMessageReached)
                        {
                            this.handleReachedEndOfMessage(encryptionPassword);
                            return;
                        }

                        this.extractMessageBitsFromPixel(embeddedPixelColor, count);
                        count += PixelConstants.NumberOfColorChannels * this.bpcc;
                    }
                }
            }
        }

        private void handleReachedEndOfMessage(StringBuilder encryptionPassword)
        {
            if (EncryptionUsed)
            {
                DecryptedText = TextCryptography.Decrypt(encryptionPassword.ToString(),
                    this.embeddedMessage.ToString());
            }

            ExtractedText = this.embeddedMessage.ToString();
        }

        private void extractMessageBitsFromPixel(Color embeddedPixelColor, int count)
        {
            var colorInfo = ColorByteArrayConverter.GetByteArray(embeddedPixelColor);

            foreach (var color in colorInfo)
            {
                var currColor = color;

                var colorByte = new[] {currColor};
                var colorBits = new BitArray(colorByte);

                for (var j = 0; j < this.bpcc; j++)
                {
                    this.embeddedBits.Set(count, colorBits.Get(j));
                    this.handleExtractLetter(count);
                    count++;
                }
            }
        }

        private void handleExtractLetter(int count)
        {
            if (count % 8 != 7)
            {
                return;
            }

            var currIndex = 0;
            var peekBits = new BitArray(this.numberOfBitsInByte);
            for (var k = count; k >= count - 7; k--)
            {
                peekBits.Set(currIndex, this.embeddedBits.Get(k));
                currIndex++;
            }

            var extractedLetter = BinaryStringConverter.BitArrayToString(peekBits);

            if (this.peek.Length == this.numberOfBitsInByte && this.endOfEncryptionKeyReached)
            {
                this.embeddedMessage.Append(this.peek.ToString().Substring(0, 1));
                this.peek.Remove(0, 1);
            }
            else if (this.peek.Length == this.numberOfBitsInByte)
            {
                this.encryptionKey.Append(this.peek.ToString().Substring(0, 1));
                this.peek.Remove(0, 1);
            }

            this.peek.Append(extractedLetter);

            if (this.endOfEncryptionKeyReached)
            {
                this.checkForEndOfMessage();
            }
            else
            {
                this.checkForEndOfEncryptionKey();
            }
        }

        private void checkForEndOfMessage()
        {
            if (this.peek.ToString().Equals(TextMessageConstants.EndOfTextFileIndication))
            {
                this.endOfMessageReached = true;
            }
        }

        private void checkForEndOfEncryptionKey()
        {
            if (this.peek.ToString().Substring(3).Equals(TextMessageConstants.EndOfEncryptionKeyIndication))
            {
                this.encryptionKey.Append(this.peek.ToString().Substring(0, 3));
                this.peek.Remove(0, 3);
                this.endOfMessageReached = true;
                this.EncryptionKey = this.encryptionKey.ToString();
            }
        }

        #endregion
    }
}