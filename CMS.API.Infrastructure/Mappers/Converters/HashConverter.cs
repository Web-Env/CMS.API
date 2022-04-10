using AutoMapper;
using System.Security.Cryptography;
using System.Text;

namespace CMS.API.Infrastructure.Mappers.Converters
{
    public class HashConverter : IValueConverter<string, byte[]>
    {
        public byte[] Convert(string password, ResolutionContext resolutionContext)
        {
            SHA256Managed sha256Hasher = new SHA256Managed();
            UTF8Encoding encoder = new UTF8Encoding();
            return sha256Hasher.ComputeHash(encoder.GetBytes(password));
        }
    }
}
