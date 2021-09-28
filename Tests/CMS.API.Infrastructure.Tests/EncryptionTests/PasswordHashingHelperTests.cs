using CMS.API.Infrastructure.Encryption.Helpers;
using CMS.API.Tests.Consts;
using Xunit;

namespace CMS.API.Infrastructure.Tests.EncryptionTests
{
    [Trait("Category", "Unit")]
    public class PasswordHashingHelperTests
    {
        [Fact]
        public void HashPassword_ShouldReturnPasswordAsHashedString()
        {
            //Arrange
            var rootUserHashedPassword = UserConsts.RootUserHashedPassword;

            //Act
            var hashedPassword = HashingHelper.HashPassword(rootUserHashedPassword);
            var hashedPasswordMatchesPlainText = BCrypt.Net.BCrypt.Verify(rootUserHashedPassword, hashedPassword);

            //Assert
            Assert.True(hashedPasswordMatchesPlainText);
        }
    }
}
