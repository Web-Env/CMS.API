using CMS.API.Tests.Consts;
using CMS.API.UploadModels;
using System;

namespace CMS.API.Tests.Helpers
{
    public static class UserControllerHelper
    {
        public static UserUploadModel GenerateUserUploadModel()
        {
            return CreateUserUploadModelObject();   
        }

        public static UserUploadModel GenerateBadUserUploadModel()
        {
            var badUserUploadModel = CreateUserUploadModelObject();
            badUserUploadModel.UserAddress = "";
            return badUserUploadModel;
        }

        public static UserUploadModel GenerateDuplicateEmailUserUploadModel()
        {
            var badUserUploadModel = CreateUserUploadModelObject();
            badUserUploadModel.Email = UserConsts.RootUserEmail;
            return badUserUploadModel;
        }

        private static UserUploadModel CreateUserUploadModelObject()
        {
            return new UserUploadModel
            {
                UserAddress = UserConsts.DefaultAddress,
                RequesterUserId = UserConsts.RootUserIdEncrypted,
                Email = "noreply@webenv.io",
                Password = UserConsts.TestUserHashedPassword,
                FirstName = UserConsts.TestUserFirstName,
                LastName = UserConsts.TestUserLastName,
                IsAdmin = false,
                CreatedOn = DateTime.Now,
                CreatedBy = UserConsts.RootUserIdEncrypted,
                LastUpdatedOn = DateTime.Now,
                LastUpdatedBy = UserConsts.RootUserIdEncrypted
            };
        }
    }
}
