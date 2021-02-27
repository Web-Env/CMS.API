using CMS.API.Infrastructure.Encryption.Helpers;
using CMS.API.Tests.Consts;
using System.Text;
using Xunit;

namespace CMS.API.Infrastructure.Tests.EncryptionTests
{
    [Trait("Category", "Unit")]
    public class PasswordHashingHelperTests
    {
        [Fact]
        public void HashPassword_ShouldReturnPasswordAsSHA256String()
        {
            //Arrange
            var rootUserPlainTextPassword = UserConsts.RootUserPlainTextPassword;
            var rootUserHashedPassword = UserConsts.RootUserHashedPassword.ToLower();

            //Act
            var hashedPassword = PasswordHashingHelper.HashPassword(rootUserPlainTextPassword);
            var hashedPasswordString = new StringBuilder();
            foreach(byte b in hashedPassword)
            {
                hashedPasswordString.Append(b.ToString("x2"));
            }

            //Assert
            Assert.Equal(rootUserHashedPassword, hashedPasswordString.ToString());
        }
    }
}
