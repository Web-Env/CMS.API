using System.Security.Cryptography;
using System.Text;

namespace CMS.API.Infrastructure.Encryption.Helpers
{
    public static class PasswordHashingHelper
    {
        public static byte[] HashPassword(string password)
        {
            SHA256Managed sha256Hasher = new SHA256Managed();
            UTF8Encoding encoder = new UTF8Encoding();
            return sha256Hasher.ComputeHash(encoder.GetBytes(password));
        }
    }
}
