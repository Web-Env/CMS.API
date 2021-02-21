using AutoMapper;
using System;
using System.Security.Cryptography;
using System.Text;

namespace CMS.API.Mappers.Converters
{
    public class HashConverter : IValueConverter<string, byte[]>
    {
        public byte[] Convert(string password, ResolutionContext resolutionContext)
        {
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
            UTF8Encoding encoder = new UTF8Encoding();
            return md5Hasher.ComputeHash(encoder.GetBytes(password));
        }
    }
}
