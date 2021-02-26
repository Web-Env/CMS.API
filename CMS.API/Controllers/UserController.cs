using AutoMapper;
using CMS.API.Exceptions;
using CMS.API.Infrastructure.Settings;
using CMS.API.Mailer;
using CMS.API.Mailer.Helpers;
using CMS.API.UploadModels;
using CMS.Domain.Entities;
using CMS.Domain.Enums;
using CMS.Domain.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
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
        private readonly EmailService _emailService;

        public UserController(IRepositoryManager repositoryManager,
                              IMapper mapper,
                              IOptions<SmtpSettings> smtpSettings) : base(repositoryManager, mapper)
        {
            _emailService = new EmailService(smtpSettings.Value);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Post(UserUploadModel user)
        {
            var existingEmail = await RepositoryManager.UserRepository.FindAsync(u => u.Email == user.Email);
            if (existingEmail.Any())
            {
                return BadRequest(
                    new EmailAlreadyRegisteredException(
                        "A User with this email address already exists",
                        "A User with this email address already exists"
                    )
                );
            }

            var newUser = MapUploadModelToEntity<User>(user);

            var registeredUser = await RepositoryManager.UserRepository.AddAsync(newUser);
            var auditLog = await LogAction(UserActionCategory.User, UserAction.Create, Guid.Parse(user.RequesterUserId), user.UserAddress, DateTime.Now);
            var passwordSet = await GeneratePasswordSetLink(registeredUser.Id, user.UserAddress);
            
            try
            {
                await SendWelcomeEmail(registeredUser, passwordSet.ResetIdentifier);
            }
            catch(Exception err)
            {
                await RevertUserCreation(registeredUser, auditLog, passwordSet);
                return BadRequest(
                    new EmailDoesNotExistException(
                        "This Email address does not exist",
                        err.Message
                    )
                );
            }

            return Ok();
        }

        private async Task SendWelcomeEmail(User registeredUser, string passwordSetLink)
        {
            await _emailService.SendEmail(
                $"{registeredUser.FirstName} {registeredUser.LastName}",
                registeredUser.Email,
                "Welcome",
                EmailCreationHelper.NewUserCreateEmail(
                    $"{registeredUser.FirstName} {registeredUser.LastName}",
                    registeredUser.Email,
                    passwordSetLink
                )
            );
        }

        private async Task RevertUserCreation(User registeredUser, AuditLog auditLog, PasswordReset passwordSet)
        {
            await RepositoryManager.UserRepository.RemoveAsync(registeredUser);
            await RepositoryManager.AuditLogRepository.RemoveAsync(auditLog);
            await RepositoryManager.PasswordResetRepository.RemoveAsync(passwordSet);
        }
    }
}
