using CMS.API.Tests.Consts;
using CMS.API.UploadModels;

namespace CMS.API.Services.Tests.Helpers
{
    public static class AuthenticationRequestHelper
    {
        public static AuthenticationRequest CreateRootUserAuthenticationRequest()
        {
            return new AuthenticationRequest
            {
                UserAddress = UserConsts.DefaultAddress,
                EmailAddress = UserConsts.RootUserEmail,
                Password = UserConsts.RootUserHashedPassword
            };
        }
        public static AuthenticationRequest CreateTestUserAuthenticationRequest()
        {
            return new AuthenticationRequest
            {
                UserAddress = UserConsts.DefaultAddress,
                EmailAddress = UserConsts.TestUserEmail,
                Password = UserConsts.TestUserHashedPassword
            };
        }
    }
}
