using AutoMapper;
using CMS.API.Infrastructure.Exceptions;
using CMS.API.Services.Authentication;
using CMS.API.UploadModels;
using CMS.Domain.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CMS.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class AuthController : CustomControllerBase
    {
        private readonly AuthenticationService _authenticationService;
        public AuthController(IRepositoryManager repositoryManager,
                              IMapper mapper,
                              AuthenticationService authenticationService) : base(repositoryManager, mapper)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost("auth")]
        [AllowAnonymous]
        public async Task<IActionResult> Authenticate(AuthenticationRequest authenticationRequest)
        {
            try
            {
                var response = await _authenticationService.AuthenticateAsync(authenticationRequest, RepositoryManager.UserRepository);

                return Ok(response);
            }
            catch(AuthenticationException authException)
            {
                return BadRequest(authException);
            }
        }

        [HttpPost("validate")]
        public IActionResult Validate()
        {
            return Ok();
        }
    }
}
