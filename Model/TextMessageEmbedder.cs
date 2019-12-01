using System;
using System.Threading.Tasks;

namespace GroupNStegafy.Model
{
    /// <summary>
    ///     Stores information for the TextMessageEmbedder class
    /// </summary>
    /// <seealso cref="GroupNStegafy.Model.Embedder" />
    public class TextMessageEmbedder : Embedder
    {
        #region Methods

        //TODO
        /// <summary>
        ///     Embeds the text message in image.
        /// </summary>
        /// <param name="messagePixels">The message pixels.</param>
        /// <param name="messageImageWidth">Width of the message image.</param>
        /// <param name="messageImageHeight">Height of the message image.</param>
        /// <param name="sourceImageWidth">Width of the source image.</param>
        /// <param name="sourceImageHeight">Height of the source image.</param>
        /// <param name="encryptionIsSelected">if set to <c>true</c> [encryption is selected].</param>
        /// <param name="bpcc">The BPCC.</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public override Task EmbedMessageInImage(byte[] messagePixels, uint messageImageWidth, uint messageImageHeight,
            uint sourceImageWidth,
            uint sourceImageHeight, bool encryptionIsSelected, int bpcc)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}