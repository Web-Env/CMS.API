using CMS.API.DownloadModels.Auth;
using CMS.API.Infrastructure.Exceptions;
using CMS.API.Services.Authentication;
using CMS.API.Services.Tests.Helpers;
using CMS.API.Tests;
using CMS.API.Tests.Funcs;
using System.Threading.Tasks;
using Xunit;

namespace CMS.API.Services.Tests.AuthenticationTests
{
    [Trait("Category", "Unit")]
    public class AuthenticationServiceTests : ServiceTestBase
    {
        private readonly AuthenticationService _authenticationService;
        public AuthenticationServiceTests(DatabaseFixture databaseFixture) : base(databaseFixture)
        {
            _authenticationService = new AuthenticationService();
        }

        [Fact]
        public async Task AuthenticateAsync_ShouldReturnAuthResponseIfUserExistsAndCredentialsValid()
        {
            //Arrange
            var context = NewContext();
            var authenticationRequest = AuthenticationRequestHelper.CreateRootUserAuthenticationRequest();
            await UserFunc.CreateRootUser(context);

            //Act
            var authenticationResponse = await _authenticationService.AuthenticateAsync(
                authenticationRequest,
                RepositoryManager.UserRepository
            );

            //Assert
            Assert.NotNull(authenticationResponse);
            Assert.IsType<AuthenticationResponse>(authenticationResponse);
        }

        [Fact]
        public async Task AuthenticateAsync_ShouldThrowExceptionIfUserExistsAndCredentialsInvalid()
        {
            //Arrange
            var context = NewContext();
            var authenticationRequest = AuthenticationRequestHelper.CreateRootUserAuthenticationRequest();
            authenticationRequest.Password = "";
            await UserFunc.CreateRootUser(context);

            //Assert
            await Assert.ThrowsAsync<AuthenticationException>(async() => await _authenticationService.AuthenticateAsync(
                authenticationRequest,
                RepositoryManager.UserRepository
            ));
        }

        [Fact]
        public async Task AuthenticateAsync_ShouldThrowExceptionIfUserDoesNotExist()
        {
            //Arrange
            var authenticationRequest = AuthenticationRequestHelper.CreateTestUserAuthenticationRequest();

            //Assert
            await Assert.ThrowsAsync<AuthenticationException>(async () => await _authenticationService.AuthenticateAsync(
                authenticationRequest,
                RepositoryManager.UserRepository
            ));
        }
    }
}
