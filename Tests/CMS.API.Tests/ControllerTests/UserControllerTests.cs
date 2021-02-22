using AutoMapper;
using CMS.API.Controllers;
using CMS.API.Tests.Funcs;
using CMS.API.Tests.Helpers;
using Microsoft.AspNetCore.Mvc;
using Nito.AsyncEx;
using System.Threading.Tasks;
using Xunit;

namespace CMS.API.Tests.ControllerTests
{
    [Trait("Category", "Unit")]
	public class UserControllerTests : ControllerTestBase
	{
		private readonly UserController _userController;
		public UserControllerTests(DatabaseFixture fixture, IMapper mapper) : base(fixture)
		{
			_userController = new UserController(RepositoryManager, mapper);
			AsyncContext.Run(() => UserFunc.CreateRootUser(GetContext()));
		}

		[Fact]
		public async Task Post_WithGoodModel_ShouldReturnOkResult()
        {
			//Arrange
			var newUser = UserControllerHelper.GenerateUserUploadModel();

			//Act
			var result = await _userController.Post(newUser);

			//Assert
			Assert.NotNull(result);
			Assert.IsNotType<BadRequestObjectResult>(result);
			Assert.IsType<OkResult>(result);
        }
	}
}
