using System;
using System.Security.Cryptography;
using System.IO;
using System.Text;

namespace Kwwika.Common.Cryptography
{

    /// <summary>

    /// SymmCrypto is a wrapper of System.Security.Cryptography.SymmetricAlgorithm classes

    /// and simplifies the interface. It supports customized SymmetricAlgorithm as well.

    /// </summary>

    public class CryptographyService
    {
        SymmetricAlgorithm _rijaendel;

        public CryptographyService()
        {
            _rijaendel = RijndaelManaged.Create();
            _rijaendel.IV = new Byte[] { 56, 151, 249, 160, 183, 47, 5, 42, 90, 5, 207, 241, 11, 166, 166, 173 };
            _rijaendel.Key = new Byte[] { 214, 145, 104, 41, 148, 129, 139, 16, 224, 38, 40, 15, 5, 254, 217, 193, 146, 43, 187, 174, 132, 181, 220, 211, 228, 181, 153, 173, 239, 194, 45, 253 };
        }

        /// <summary>
        /// Encrypts the string and returns a base64 encoded encrypted string.
        /// </summary>
        /// <param name="clearText">The clear text.</param>
        /// <returns></returns>
        public string EncryptString(string clearText)
        {
            byte[] clearTextBytes = Encoding.UTF8.GetBytes(clearText);
            byte[] encrypted = _rijaendel.CreateEncryptor().TransformFinalBlock(clearTextBytes, 0, clearTextBytes.Length);
            return Convert.ToBase64String(encrypted);
        }

        /// <summary>
        /// Decrypts the base64 encrypted string and returns the cleartext.
        /// </summary>
        /// <param name="encryptedEncodedText">The clear text.</param>
        /// <exception type="System.Security.Cryptography.CryptographicException">Thrown the string to be decrypted
        /// was encrypted using a different encryptor (for example, if we recompile and
        /// receive an old string).</exception>
        /// <returns></returns>
        public string DecryptString(string encryptedEncodedText)
        {
            string decryptedString = null;
            try
            {
                byte[] encryptedBytes = Convert.FromBase64String(encryptedEncodedText);
                byte[] decryptedBytes = _rijaendel.CreateDecryptor().TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                decryptedString =  Encoding.UTF8.GetString(decryptedBytes);
            }
            catch (FormatException fe)
            {
                throw;// new CaptchaExpiredException("Encrypted encoded text '" + encryptedEncodedText + "' was not valid.", fe);
            }
            catch (CryptographicException e)
            {
                throw;// new CaptchaExpiredException("Captcha image expired, probably due to recompile making the key out of synch.", e);
            }

            return decryptedString;
        }

    }
}