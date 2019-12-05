using System;

namespace GroupNStegafy.Model
{
    public class VCipher
    {
        private const int ENCRYPT = 1;
        private const int DECRYPT = -1;
        private const int AsciiLetterA = 65;
        private const int LettersInAlphabet = 26;

        private string handleEncryption(string input, string password, int encryptOrDecrypt)
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

        public string Encrypt(string input, string password)
        {
            return this.handleEncryption(input, password, ENCRYPT);
        }

        public string Decrypt(string input, string password)
        {
            return this.handleEncryption(input, password, DECRYPT);
        }
    }
}