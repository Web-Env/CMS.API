using CMS.API.Infrastructure.Encryption;
using CMS.API.Tests.Consts;
using System;
using Xunit;

namespace CMS.API.Infrastructure.Tests.EncryptionTests
{
    [Trait("Category", "Unit")]
    public class DecryptionServiceTests
    {
        [Fact]
        public void DecryptUserId_ShouldDecryptEncryptedUserId()
        {
            //Arrange
            var encryptedUserId = UserConsts.RootUserIdEncrypted;
            var rootUserId = Guid.Parse(UserConsts.RootUserId);

            //Act
            var decryptedUserId = DecryptionService.DecryptUserId(encryptedUserId);

            //Assert
            Assert.Equal(rootUserId, decryptedUserId);
        }

        [Fact]
        public void DecryptString_ShouldDecryptEncryptedString()
        {
            //Arrange
            var encryptedString = EncryptionConsts.EncryptedString;

            //Act
            var decryptedString = DecryptionService.DecryptString(encryptedString);

            //Assert
            Assert.Equal(EncryptionConsts.StringToBeEncrypted, decryptedString);
        }
    }
}
