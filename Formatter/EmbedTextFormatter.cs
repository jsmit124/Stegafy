using GroupNStegafy.Constants;
using GroupNStegafy.Cryptography;

namespace GroupNStegafy.Formatter
{
    /// <summary>
    ///     Stores information for EmbedTextFormatter class
    /// </summary>
    public static class EmbedTextFormatter
    {
        #region Methods

        /// <summary>
        ///     Formats the text for embedding.
        /// </summary>
        /// @Precondition none
        /// @Postcondition none
        /// <param name="text">The text.</param>
        /// <returns>String value of the formatted text</returns>
        public static string FormatTextForEmbedding(string text)
        {
            return EmbeddingStringFormatter.FormatForEmbedding(text) + TextMessageConstants.EndOfTextFileIndication;
        }

        /// <summary>
        ///     Formats the encrypted text for embedding.
        /// </summary>
        /// @Precondition none
        /// @Postcondition none
        /// <param name="password">The password.</param>
        /// <param name="text">The text.</param>
        /// <returns>The formatted encrypted text</returns>
        public static string FormatEncryptedTextForEmbedding(string password, string text)
        {
            text = EmbeddingStringFormatter.FormatForEmbedding(text);

            return password + TextMessageConstants.EndOfEncryptionKeyIndication +
                   TextCryptography.Encrypt(password, text) + TextMessageConstants.EndOfTextFileIndication;
        }

        #endregion
    }
}