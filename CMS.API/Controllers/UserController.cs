using AutoMapper;
using CMS.API.Infrastructure.Settings;
using CMS.API.Mailer.Helpers;
using CMS.API.Models.User;
using CMS.API.UploadModels.User;
using CMS.Domain.Entities;
using CMS.Domain.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using WebEnv.Util.Mailer.Settings;

namespace CMS.API.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("[controller]")]
    public class UserController : CustomControllerBase
    {
        private SmtpSettings _smtpSettings;
        private readonly EmailSettings _emailSettings;
        private readonly OrganisationSettings _organisationSettings;

        public UserController(IRepositoryManager repositoryManager,
                              IMapper mapper,
                              IOptions<SmtpSettings> smtpSettings,
                              IOptions<OrganisationSettings> organisationSettings) : base(repositoryManager, mapper)
        {
            _smtpSettings = smtpSettings.Value;
            _organisationSettings = organisationSettings.Value;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Post(UserUploadModel user)
        {
            var newUser = MapUploadModelToEntity<User>(user);

            await UserModel.CreateNewUserAsync(
                newUser,
                ExtractRequesterAddress(),
                RepositoryManager,
                _smtpSettings,
                _emailSettings);

            return Ok();
        }

        private async Task RevertUserCreation(User registeredUser, AuditLog auditLog, PasswordReset passwordSet)
        {
            await RepositoryManager.UserRepository.RemoveAsync(registeredUser);
            await RepositoryManager.AuditLogRepository.RemoveAsync(auditLog);
            await RepositoryManager.PasswordResetRepository.RemoveAsync(passwordSet);
        }
    }
}
