using CMS.API.DownloadModels;
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
        private AuthenticationService _authenticationService;
        public AuthenticationServiceTests(DatabaseFixture databaseFixture) : base(databaseFixture)
        {
            _authenticationService = new AuthenticationService();
        }

        [Fact]
        public async Task AuthenticateAsync_ShouldReturnAuthResponseIfUserExistsAndCredentialsValid()
        {
            //Arrange
            var context = NewContext();
            var authenticationRequest = AuthenticationRequestHelper.CreateAuthenticationRequest();
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
    }
}
