using CMS.API.Tests.Consts;
using CMS.API.UploadModels.User;

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
            badUserUploadModel.Email = "";
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
                Email = "noreply@webenv.io",
                FirstName = UserConsts.TestUserFirstName,
                LastName = UserConsts.TestUserLastName,
                IsAdmin = false
            };
        }
    }
}
