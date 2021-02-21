using CMS.API.UploadModels;
using CMS.Domain.Entities;
using CMS.Domain.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CMS.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class UserController : CustomControllerBase
    {
        public UserController(IRepositoryManager repositoryManager) : base(repositoryManager) { }


        [HttpPost]
        [AllowAnonymous] //TODO: Remove AllowAnonymous annotation
        public async Task<IActionResult> Post(UserUploadModel user)
        {
            var existingEmail = await RepositoryManager.UserRepository.FindAsync(u => u.Email == user.Email);
            if (existingEmail.Any())
            {
                return BadRequest("A User with this email address already exists");
            }

            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
            Byte[] hashedPassword;
            UTF8Encoding encoder = new UTF8Encoding();
            hashedPassword = md5Hasher.ComputeHash(encoder.GetBytes(user.Password));

            var newUser = new User()
            {
                Id = Guid.NewGuid(),
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Password = hashedPassword,
                IsAdmin = user.IsAdmin,
                CreatedOn = DateTime.Now
            };

            await RepositoryManager.UserRepository.AddAsync(newUser);

            return Ok();
        }
    }
}
