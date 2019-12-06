using System;

namespace GroupNStegafy.Cryptography
{
    public static class TextCryptography
    {
        private const int ENCRYPT = 1;
        private const int DECRYPT = -1;
        private const int AsciiLetterA = 65;
        private const int LettersInAlphabet = 26;

        private static string handleEncryption(string password, string input, int encryptOrDecrypt)
        {
            var passwordCount = 0;
            var encryptedMessage = "";

            input = input.ToUpper();
            password = password.ToUpper();

            foreach (var currChar in input)
            {
                var tmp = currChar - AsciiLetterA + encryptOrDecrypt * (password[passwordCount] - AsciiLetterA);

                if (tmp < 0)
                {
                    tmp += LettersInAlphabet;
                }

                encryptedMessage += Convert.ToChar(AsciiLetterA + (tmp % LettersInAlphabet));

                if (++passwordCount == password.Length)
                {
                    passwordCount = 0;
                }
            }

            return encryptedMessage;
        }

        public static string Encrypt(string password, string input)
        {
            return handleEncryption(password, input, ENCRYPT);
        }

        public static string Decrypt(string password, string input)
        {
            return handleEncryption(password, input, DECRYPT);
        }
    }
}