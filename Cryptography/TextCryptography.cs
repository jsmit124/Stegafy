using System;
using System.Text;

namespace GroupNStegafy.Cryptography
{
    /// <summary>
    ///     Stores information for the TextCryptography class
    /// </summary>
    public static class TextCryptography
    {
        #region Data members

        private const int ENCRYPT = 1;
        private const int DECRYPT = -1;
        private const int AsciiLetterA = 65;
        private const int LettersInAlphabet = 26;

        #endregion

        #region Methods

        private static string handleEncryption(string password, string input, int encryptOrDecrypt)
        {
            var passwordCount = 0;
            var encryptedMessage = new StringBuilder();

            input = input.ToUpper();

            foreach (var currChar in input)
            {
                var tmp = currChar - AsciiLetterA + encryptOrDecrypt * (password[passwordCount] - AsciiLetterA);

                if (tmp < 0)
                {
                    tmp += LettersInAlphabet;
                }

                encryptedMessage.Append(Convert.ToChar(AsciiLetterA + tmp % LettersInAlphabet));

                if (++passwordCount == password.Length)
                {
                    passwordCount = 0;
                }
            }

            return encryptedMessage.ToString();
        }

        /// <summary>
        ///     Encrypts the specified password.
        /// </summary>
        /// @Precondition password.Length != 0 && input.Length != 0
        /// @Postcondition none
        /// <param name="password">The password.</param>
        /// <param name="input">The input.</param>
        /// <returns>The encrypted message</returns>
        public static string Encrypt(string password, string input)
        {
            return handleEncryption(password, input, ENCRYPT);
        }

        /// <summary>
        ///     Decrypts the specified password.
        /// </summary>
        /// @Precondition password.Length != 0 && input.Length != 0
        /// @Postcondition none
        /// <param name="password">The password.</param>
        /// <param name="input">The input.</param>
        /// <returns>The decrypted message</returns>
        public static string Decrypt(string password, string input)
        {
            return handleEncryption(password, input, DECRYPT);
        }

        #endregion
    }
}