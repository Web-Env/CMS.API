﻿using AutoMapper;
using CMS.API.Infrastructure.Consts;
using CMS.API.Infrastructure.Encryption;
using CMS.API.Infrastructure.Exceptions;
using CMS.API.Infrastructure.Settings;
using CMS.API.Mailer;
using CMS.API.Mailer.Helpers;
using CMS.API.Models;
using CMS.API.UploadModels.User;
using CMS.Domain.Entities;
using CMS.Domain.Enums;
using CMS.Domain.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Threading.Tasks;
using WebEnv.Util.Mailer.Settings;

namespace CMS.API.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("[controller]")]
    public class UserController : CustomControllerBase
    {
        private readonly EmailService _emailService;
        private readonly OrganisationSettings _organisationSettings;

        public UserController(IRepositoryManager repositoryManager,
                              IMapper mapper,
                              IOptions<Infrastructure.Settings.SmtpSettings> smtpSettings,
                              IOptions<OrganisationSettings> organisationSettings) : base(repositoryManager, mapper)
        {
            _emailService = new EmailService(smtpSettings.Value);
            _organisationSettings = organisationSettings.Value;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Post(UserUploadModel user)
        {
            //user = DecryptIncomingData(user);
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

            newUser.Password = BCrypt.Net.BCrypt.HashPassword(newUser.Password, workFactor: 12);
            newUser.UserSecret = EncryptionService.EncryptString(ModelHelpers.GenerateUniqueIdentifier(IdentifierConsts.IdentifierLength));

            var registeredUser = await RepositoryManager.UserRepository.AddAsync(newUser);
            var auditLog = await LogAction(UserActionCategory.User, UserAction.Create, Guid.Parse(user.RequesterUserId), DateTime.Now);
            var passwordSet = await GeneratePasswordSetLink(registeredUser.Id, ExtractUserAddress());
            
            try
            {
                var emailDelivered = await SendWelcomeEmail(registeredUser, passwordSet.Identifier)
                                            .ConfigureAwait(false);

                if (!emailDelivered)
                {
                    throw (new Exception());
                }
            }
            catch(Exception err)
            {
                await RevertUserCreation(registeredUser, auditLog, passwordSet).ConfigureAwait(false);
                return BadRequest(
                    new EmailDoesNotExistException(
                        "This Email address does not exist",
                        err.Message
                    )
                );
            }

            return Ok();
        }

        private async Task<bool> SendWelcomeEmail(User registeredUser, string passwordSetLink)
        {
            return await _emailService.SendEmail(
                $"{registeredUser.FirstName} {registeredUser.LastName}",
                registeredUser.Email,
                $"Welcome to {_organisationSettings.OrganisationName}",
                EmailCreationHelper.NewUserCreateEmail(
                    registeredUser.FirstName,
                    registeredUser.LastName,
                    _organisationSettings.OrganisationName,
                    _organisationSettings.OrganisationUrl,
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
