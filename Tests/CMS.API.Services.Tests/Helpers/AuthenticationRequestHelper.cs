using CMS.API.Tests.Consts;
using CMS.API.UploadModels;

namespace CMS.API.Services.Tests.Helpers
{
    public static class AuthenticationRequestHelper
    {
        public static AuthenticationRequest CreateAuthenticationRequest()
        {
            return new AuthenticationRequest
            {
                UserAddress = UserConsts.DefaultAddress,
                EmailAddress = UserConsts.RootUserEmail,
                Password = UserConsts.RootUserHashedPassword
            };
        }
    }
}
