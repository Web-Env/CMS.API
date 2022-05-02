using CMS.API.Controllers;
using CMS.API.Infrastructure.Settings;
using Microsoft.Extensions.Options;
using WebEnv.Util.Mailer.Settings;
using Xunit;

namespace CMS.API.Tests.ControllerTests
{
    [Trait("Category", "Unit")]
    public class UserControllerTests : ControllerTestBase
    {
        private readonly UserController _userController;
        //public UserControllerTests(DatabaseFixture fixture) : base(fixture)
        //{
        //    IOptions<SmtpSettings> smtpSettings = Options.Create(SmtpSettings);
        //    IOptions<EmailSettings> emailSettings = Options.Create(EmailSettings);
        //    IOptions<OrganisationSettings> organisationSettings = Options.Create(OrganisationSettings);
        //    _userController = new UserController(CreateTestRepositoryContext(), Mapper, smtpSettings, emailSettings, organisationSettings);
        //}

        //[Fact]
        //public async Task Post_WithBadModel_ShouldReturnBadResult()
        //{
        //    //Arrange
        //    var newUser = UserControllerHelper.GenerateBadUserUploadModel();

        //    //Act
        //    var result = await _userController.Post(newUser);

        //    //Assert
        //    Assert.NotNull(result);
        //    Assert.IsNotType<OkResult>(result);
        //    Assert.IsType<BadRequestObjectResult>(result);
        //}

        //[Fact]
        //public async Task Post_WithExistingEmail_ShouldReturnBadResult()
        //{
        //    //Arrange
        //    var newUser = UserControllerHelper.GenerateDuplicateEmailUserUploadModel();

        //    //Act
        //    var result = await _userController.Post(newUser);

        //    //Assert
        //    Assert.NotNull(result);
        //    Assert.IsNotType<OkResult>(result);
        //    Assert.IsType<BadRequestObjectResult>(result);
        //}

        //[Fact]
        //public async Task Post_WithGoodModel_ShouldReturnOkResult()
        //{
        //    //Arrange
        //    var newUser = UserControllerHelper.GenerateUserUploadModel();

        //    //Act
        //    var result = await _userController.Post(newUser);

        //    //Assert
        //    Assert.NotNull(result);
        //    Assert.IsNotType<BadRequestObjectResult>(result);
        //    Assert.IsType<OkResult>(result);
        //}
    }
}
