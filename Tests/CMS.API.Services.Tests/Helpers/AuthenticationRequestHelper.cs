﻿using CMS.API.Tests.Consts;
using CMS.API.UploadModels.Auth;

namespace CMS.API.Services.Tests.Helpers
{
    public static class AuthenticationRequestHelper
    {
        public static AuthenticationRequest CreateRootUserAuthenticationRequest()
        {
            return new AuthenticationRequest
            {
                UserAddress = UserConsts.DefaultAddress,
                Email = UserConsts.RootUserEmail,
                Password = UserConsts.RootUserHashedPassword
            };
        }
        public static AuthenticationRequest CreateTestUserAuthenticationRequest()
        {
            return new AuthenticationRequest
            {
                UserAddress = UserConsts.DefaultAddress,
                Email = UserConsts.TestUserEmail,
                Password = UserConsts.TestUserHashedPassword
            };
        }
    }
}
