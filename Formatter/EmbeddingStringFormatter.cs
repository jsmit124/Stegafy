using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Appointments.AppointmentsProvider;

namespace GroupNStegafy.Formatter
{
    public static class EmbeddingStringFormatter
    {
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
    }
}
