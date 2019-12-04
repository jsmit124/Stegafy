using System;

namespace GroupNStegafy.Model
{
    public class VCipher
    {
        private const int ENCRYPT = 1;
        private const int DECRYPT = -1;

        private string handleEncryption(string input, string password, int type)
        {
            var passwordCount = 0;
            var encryptedMessage = "";

            input = input.ToUpper();
            password = password.ToUpper();

            foreach (var currChar in input)
            {
                var tmp = currChar - 65 + type * (password[passwordCount] - 65);

                if (tmp < 0)
                {
                    tmp += 26;
                }

                encryptedMessage += Convert.ToChar(65 + (tmp % 26));

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