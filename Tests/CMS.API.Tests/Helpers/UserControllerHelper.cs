using CMS.API.Tests.Consts;
using CMS.API.UploadModels;
using System;

namespace CMS.API.Tests.Helpers
{
    public static class UserControllerHelper
    {
        public static UserUploadModel GenerateUserUploadModel()
        {
            return new UserUploadModel
            {
                UserAddress = UserConsts.DefaultAddress,
                RequesterUserId = UserConsts.RootUserId,
                Email = "tester.testerson@testing.com",
                Password = UserConsts.TestUserPassword,
                FirstName = UserConsts.TestUserFirstName,
                LastName = UserConsts.TestUserLastName,
                IsAdmin = false,
                CreatedOn = DateTime.Now,
                CreatedBy = Guid.Empty,
                LastUpdatedOn = DateTime.Now,
                LastUpdatedBy = Guid.Empty
            };
        }

        public static UserUploadModel GenerateBadUserUploadModel()
        {
            return new UserUploadModel
            {
                UserAddress = "",
                RequesterUserId = UserConsts.RootUserId,
                Email = "tester.testerson@testing.com",
                Password = UserConsts.TestUserPassword,
                FirstName = UserConsts.TestUserFirstName,
                LastName = UserConsts.TestUserLastName,
                IsAdmin = false,
                CreatedOn = DateTime.Now,
                CreatedBy = Guid.Empty,
                LastUpdatedOn = DateTime.Now,
                LastUpdatedBy = Guid.Empty
            };
        }

        public static UserUploadModel GenerateDuplicateEmailUserUploadModel()
        {
            return new UserUploadModel
            {
                UserAddress = UserConsts.DefaultAddress,
                RequesterUserId = UserConsts.RootUserId,
                Email = UserConsts.RootUserEmail,
                Password = UserConsts.TestUserPassword,
                FirstName = UserConsts.TestUserFirstName,
                LastName = UserConsts.TestUserLastName,
                IsAdmin = false,
                CreatedOn = DateTime.Now,
                CreatedBy = Guid.Empty,
                LastUpdatedOn = DateTime.Now,
                LastUpdatedBy = Guid.Empty
            };
        }
    }
}
