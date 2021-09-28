using AutoMapper;
using CMS.API.Infrastructure.Encryption.Helpers;
using CMS.API.Infrastructure.Exceptions;
using CMS.API.Infrastructure.Settings;
using CMS.API.Models.User;
using CMS.API.UploadModels.User;
using CMS.Domain.Entities;
using CMS.Domain.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
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
                              IOptions<EmailSettings> emailSettings,
                              IOptions<OrganisationSettings> organisationSettings) : base(repositoryManager, mapper)
        {
            _smtpSettings = smtpSettings.Value;
            _emailSettings = emailSettings.Value;
            _organisationSettings = organisationSettings.Value;
        }

        [HttpPost("CreateUser")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateUser(UserUploadModel user)
        {
            try
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
            catch (EmailAlreadyRegisteredException err)
            {
                return BadRequest(new EmailAlreadyRegisteredException(err.ErrorMessage, err.ErrorData));
            }
            catch (Exception err)
            {
                return Problem();
            }
        }

        [HttpPost("Verify")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyUser(string verificationIdentifier)
        {
            try
            {
                await UserModel.VerifyUserAsync(
                    verificationIdentifier,
                    ExtractRequesterAddress(),
                    RepositoryManager);

                return Ok();
            }
            catch (InvalidTokenException err)
            {
                return BadRequest(new InvalidTokenException(err.InvalidTokenType, err.ErrorMessage));
            }
            catch (Exception err)
            {
                return Problem();
            }
        }

        [HttpGet("Verify/Validate")]
        [AllowAnonymous]
        public async Task<IActionResult> ValidateVerificationIdentifier(string verificationIdentifier)
        {
            try
            {
                var verificationIdentifierIsValid = await UserModel.ValidateVerificationIdentifierAsync(
                    HashingHelper.HashIdentifier(verificationIdentifier),
                    RepositoryManager.UserVerificationRepository);

                if (verificationIdentifierIsValid)
                {
                    return Ok();
                }

                return BadRequest();
            }
            catch (InvalidTokenException err)
            {
                return BadRequest(new InvalidTokenException(err.InvalidTokenType, err.ErrorMessage));
            }
            catch (Exception err)
            {
                return Problem();
            }
        }

        [HttpPost("Verify/New")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateNewVerification(string email)
        {
            try
            {
                await UserModel.CreateNewVerficationAsync(
                    email,
                    ExtractRequesterAddress(),
                    RepositoryManager,
                    _smtpSettings,
                    _emailSettings);

                return Ok();
            }
            catch (UserAlreadyVerifiedException err)
            {
                return BadRequest(new UserAlreadyVerifiedException(err.ErrorMessage, err.ErrorData));
            }
            catch (Exception err)
            {
                return Problem();
            }
        }

        [HttpPost("ForgotPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(PasswordResetUploadModel passwordResetUploadModel)
        {
            try
            {
                await UserModel.ResetPasswordAsync(
                    passwordResetUploadModel.PasswordResetToken,
                    passwordResetUploadModel.NewPassword,
                    ExtractRequesterAddress(),
                    RepositoryManager);

                return Ok();
            }
            catch (InvalidTokenException err)
            {
                return BadRequest(new InvalidTokenException(err.InvalidTokenType, err.ErrorMessage));
            }
            catch (Exception err)
            {
                return Problem();
            }
        }

        [HttpGet("ForgotPassword/Validate")]
        [AllowAnonymous]
        public async Task<IActionResult> ValidatePasswordResetToken(string passwordResetToken)
        {
            try
            {
                var passwordResetTokenIsValid = await UserModel.ValidatePasswordResetTokenAsync(
                    passwordResetToken,
                    RepositoryManager.PasswordResetRepository);

                if (passwordResetTokenIsValid)
                {
                    return Ok();
                }

                return BadRequest();
            }
            catch (InvalidTokenException err)
            {
                return BadRequest(new InvalidTokenException(err.InvalidTokenType, err.ErrorMessage));
            }
            catch (Exception err)
            {
                return Problem();
            }
        }

        [HttpPost("ForgotPassword/New")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateNewResetPassword(string emailAddress)
        {
            try
            {
                await UserModel.CreateNewResetPasswordAsync(
                    emailAddress,
                    ExtractRequesterAddress(),
                    RepositoryManager,
                    _smtpSettings,
                    _emailSettings);

                return Ok();
            }
            catch (Exception err)
            {
                return Problem();
            }
        }
    }
}
