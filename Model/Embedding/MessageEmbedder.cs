using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace GroupNStegafy.Model.Embedding
{
    /// <summary>
    ///     Stores information for the abstract Embedder class
    /// </summary>
    public abstract class MessageEmbedder
    {
        #region Properties

        /// <summary>
        ///     Gets or sets a value indicating whether [message too large].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [message too large]; otherwise, <c>false</c>.
        /// </value>
        public bool MessageTooLarge { get; protected set; }

        /// <summary>
        ///     Gets or sets the source image pixels.
        /// </summary>
        /// <value>The source image pixels.</value>
        public byte[] SourceImagePixels;

        /// <summary>
        ///     Gets or sets the embedded image.
        /// </summary>
        /// <value>The embedded image.</value>
        public WriteableBitmap EmbeddedImage { get; protected set; }

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="MessageEmbedder" /> class
        /// </summary>
        protected MessageEmbedder()
        {
            this.MessageTooLarge = false;
            this.SourceImagePixels = null;
            this.EmbeddedImage = null;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Embeds the message in the source image.
        /// </summary>
        /// @Precondition none
        /// @Postcondition the message is embedded into the image
        /// <param name="messageData">The message data.</param>
        /// <param name="messageImageWidth">Width of the message image.</param>
        /// <param name="messageImageHeight">Height of the message image.</param>
        /// <param name="sourceImageWidth">Width of the source image.</param>
        /// <param name="sourceImageHeight">Height of the source image.</param>
        /// <param name="encryptionIsSelected">if set to <c>true</c> [encryption is selected].</param>
        /// <param name="bpcc">The BPCC.</param>
        /// <returns>Task</returns>
        public abstract Task EmbedMessageInImage(byte[] messageData, uint messageImageWidth, uint messageImageHeight,
            uint sourceImageWidth, uint sourceImageHeight, bool encryptionIsSelected, int bpcc);

        /// <summary>
        ///     Determines whether [is first pixel] [the specified curr x].
        /// </summary>
        /// @Precondition none
        /// @Postcondition none
        /// <param name="currX">The curr x.</param>
        /// <param name="currY">The curr y.</param>
        /// <returns><c>true</c> if [is first pixel] [the specified curr x]; otherwise, <c>false</c>.</returns>
        protected bool IsFirstPixel(int currX, int currY)
        {
            return currY == 0 && currX == 0;
        }

        /// <summary>
        ///     Determines whether [is second pixel] [the specified curr x].
        /// </summary>
        /// @Precondition none
        /// @Postcondition none
        /// <param name="currX">The curr x.</param>
        /// <param name="currY">The curr y.</param>
        /// <returns><c>true</c> if [is second pixel] [the specified curr x]; otherwise, <c>false</c>.</returns>
        protected bool IsSecondPixel(int currX, int currY)
        {
            return currY == 0 && currX == 1;
        }

        /// <summary>
        ///     Sets the embedded image.
        /// </summary>
        /// @Precondition none
        /// @Postcondition none
        /// <param name="imageHeight">Height of the image.</param>
        /// <param name="imageWidth">Width of the image.</param>
        protected async Task SetEmbeddedImage(uint imageHeight, uint imageWidth)
        {
            this.EmbeddedImage = new WriteableBitmap((int) imageWidth, (int) imageHeight);
            using (var writeStream = this.EmbeddedImage.PixelBuffer.AsStream())
            {
                await writeStream.WriteAsync(this.SourceImagePixels, 0, this.SourceImagePixels.Length);
            }
        }

        #endregion
    }
}