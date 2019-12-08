using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace GroupNStegafy.Model.Extracting
{
    /// <summary>
    ///     Stores information for the MessageExtractor abstract class
    /// </summary>
    public abstract class MessageExtracter
    {
        #region Data members

        /// <summary>
        ///     The encryption used
        /// </summary>
        public bool EncryptionUsed;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the extracted image.
        /// </summary>
        /// <value>The extracted image.</value>
        public WriteableBitmap ExtractedImage { get; protected set; }

        /// <summary>
        ///     Gets or sets the decrypted image.
        /// </summary>
        /// <value>The decrypted image.</value>
        public WriteableBitmap DecryptedImage { get; protected set; }

        /// <summary>
        ///     Gets or sets the extracted text.
        /// </summary>
        /// <value>The extracted text.</value>
        public string ExtractedText { get; protected set; }

        /// <summary>
        ///     Gets or sets the decrypted text
        /// </summary>
        /// <value>The decrypted text.</value>
        public string DecryptedText { get; protected set; }

        /// <summary>
        ///     Gets or sets the encryption key.
        /// </summary>
        /// <value>The encryption key.</value>
        public string EncryptionKey { get; protected set; }

        #endregion

        #region Methods

        /// <summary>
        ///     Extracts the message from image.
        /// </summary>
        /// @Precondition none
        /// @Postcondition message is extracted from the image
        /// <param name="embeddedPixels">The embedded pixels.</param>
        /// <param name="embeddedImageWidth">Width of the embedded image.</param>
        /// <param name="embeddedImageHeight">Height of the embedded image.</param>
        /// <returns>Task</returns>
        public abstract Task ExtractMessageFromImage(byte[] embeddedPixels, uint embeddedImageWidth,
            uint embeddedImageHeight);

        /// <summary>
        ///     Determines whether [is second pixel] [the specified curr y].
        /// </summary>
        /// @Precondition none
        /// @Postcondition none
        /// <param name="currY">The curr y.</param>
        /// <param name="currX">The curr x.</param>
        /// <returns>
        ///     <c>true</c> if [is second pixel] [the specified curr y]; otherwise, <c>false</c>.
        /// </returns>
        protected static bool isSecondPixel(int currY, int currX)
        {
            return currY == 0 && currX == 1;
        }

        /// <summary>
        ///     Determines whether [is first pixel] [the specified curr y].
        /// </summary>
        /// @Precondition none
        /// @Postcondition none
        /// <param name="currY">The curr y.</param>
        /// <param name="currX">The curr x.</param>
        /// <returns>
        ///     <c>true</c> if [is first pixel] [the specified curr y]; otherwise, <c>false</c>.
        /// </returns>
        protected static bool isFirstPixel(int currY, int currX)
        {
            return currY == 0 && currX == 0;
        }

        #endregion
    }
}