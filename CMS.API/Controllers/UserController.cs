using AutoMapper;
using CMS.API.UploadModels;
using CMS.Domain.Entities;
using CMS.Domain.Enums;
using CMS.Domain.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class UserController : CustomControllerBase
    {
        public UserController(IRepositoryManager repositoryManager,
                              IMapper mapper) : base(repositoryManager, mapper) { }


        [HttpPost]
        [AllowAnonymous] //TODO: Remove AllowAnonymous annotation once login method has been developed
        public async Task<IActionResult> Post(UserUploadModel user)
        {
            var existingEmail = await RepositoryManager.UserRepository.FindAsync(u => u.Email == user.Email);
            if (existingEmail.Any())
            {
                return BadRequest("A User with this email address already exists");
            }

            var newUser = MapUploadModelToEntity<User>(user);

            await RepositoryManager.UserRepository.AddAsync(newUser);
            await LogAction(UserActionCategory.User, UserAction.Create, Guid.Parse(user.RequesterUserId), user.UserAddress, DateTime.Now);

            return Ok();
        }
    }
}
