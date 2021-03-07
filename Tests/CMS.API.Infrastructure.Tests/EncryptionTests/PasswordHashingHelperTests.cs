﻿using CMS.API.Infrastructure.Encryption.Helpers;
using CMS.API.Tests.Consts;
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
            var rootUserHashedPassword = UserConsts.RootUserHashedPassword.ToLower();

            //Act
            var hashedPassword = PasswordHashingHelper.HashPassword(rootUserHashedPassword);
            var hashedPasswordMatchesPlainText = BCrypt.Net.BCrypt.Verify(rootUserHashedPassword, hashedPassword);

            //Assert
            Assert.True(hashedPasswordMatchesPlainText);
        }
    }
}