using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace CMS.API.Infrastructure.Encryption
{
    public static class DecryptionService
    {
        public static Guid DecryptUserId(string userId)
        {
            return Guid.Parse(DecryptString(userId));
        }

        public static string DecryptString(string input)
        {
            var privateKey = FetchPrivateKey();

            var cipher = new RSACryptoServiceProvider();
            cipher.FromXmlString(privateKey);
            byte[] cipherText = Convert.FromBase64String(input);
            byte[] decryptedText = cipher.Decrypt(cipherText, false);

            return Encoding.UTF8.GetString(decryptedText);
        }

        private static string FetchPrivateKey()
        {
            return File.ReadAllText(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                "Encryption/Keys/cms_private_key.xml")
            );
        }
    }
}
