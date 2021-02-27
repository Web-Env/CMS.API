using CMS.API.Infrastructure.Encryption;
using CMS.API.Tests.Consts;
using System;
using Xunit;

namespace CMS.API.Infrastructure.Tests.EncryptionTests
{
    [Trait("Category", "Unit")]
    public class EncryptionServiceTests
    {
        [Fact]
        public void EncryptUserId_ShouldEncryptUserIdWithPublicKey()
        {
            //Arrange
            var userId = Guid.Parse(UserConsts.RootUserId);
            var rootUserId = Guid.Parse(UserConsts.RootUserId);

            //Act
            var encryptedUserId = EncryptionService.EncryptUserId(userId);
            var decryptedUserId = DecryptionService.DecryptUserId(encryptedUserId);
            //Bad practice to use other "un-tested" methods inside unit tests for a particular method but due to RSA encryption
            //protocols there's no other comparison operation that can be run to ensure the Id is encrypted

            //Assert
            Assert.Equal(rootUserId, decryptedUserId);
        }

        [Fact]
        public void EncryptString_ShouldEncryptStringWithPublicKey()
        {
            //Arrange
            var stringToBeEncrypted = EncryptionConsts.StringToBeEncrypted;

            //Act
            var encryptedString = EncryptionService.EncryptString(stringToBeEncrypted);
            var decryptedString = DecryptionService.DecryptString(encryptedString);
            //Bad practice to use other "un-tested" methods inside unit tests for a particular method but due to RSA encryption
            //protocols there's no other comparison operation that can be run to ensure the string is encrypted

            //Assert
            Assert.Equal(EncryptionConsts.StringToBeEncrypted, decryptedString);
        }
    }
}
