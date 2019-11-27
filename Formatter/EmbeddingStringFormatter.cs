using System;
using System.Linq;

namespace GroupNStegafy.Formatter
{
    /// <summary>
    ///     Stores methods for formatting strings to be embedded in an image
    /// </summary>
    public static class EmbeddingStringFormatter
    {
        #region Methods

        /// <summary>
        ///     Formats input string for embedding.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>The formatted string</returns>
        public static string FormatForEmbedding(string text)
        {
            text = removeWhiteSpace(text);
            text = removeTabCharacters(text);
            text = removeNewLineCharacters(text);
            text = removePunctuationCharacters(text);

            return text;
        }

        private static string removePunctuationCharacters(string text)
        {
            return new string(text.Where(c => !char.IsPunctuation(c)).ToArray());
        }

        private static string removeWhiteSpace(string text)
        {
            text = text.Replace(" ", string.Empty);
            return text;
        }

        private static string removeTabCharacters(string text)
        {
            text = text.Replace("\t", string.Empty);
            return text;
        }

        private static string removeNewLineCharacters(string text)
        {
            text = text.Replace(Environment.NewLine, string.Empty);
            return text;
        }

        #endregion
    }
}