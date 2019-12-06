
using GroupNStegafy.Constants;
using GroupNStegafy.Cryptography;

namespace GroupNStegafy.Formatter
{
    public static class EmbedTextFormatter
    {
        public static string FormatTextForEmbedding(string text)
        {
            return EmbeddingStringFormatter.FormatForEmbedding(text) + TextMessageConstants.EndOfTextFileIndication;
        }

        public static string FormatEncryptedTextForEmbedding(string password, string text)
        {
            text = EmbeddingStringFormatter.FormatForEmbedding(text);

            return password + TextMessageConstants.EndOfEncryptionKeyIndication +
                   TextCryptography.Encrypt(password, text) + TextMessageConstants.EndOfTextFileIndication;
        }
    }
}
