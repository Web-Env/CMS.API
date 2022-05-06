using AutoMapper;
using CMS.API.DownloadModels.User;
using CMS.API.Infrastructure.Encryption.Helpers;
using CMS.API.Infrastructure.Exceptions;
using CMS.API.Infrastructure.Settings;
using CMS.API.Models.User;
using CMS.API.UploadModels.User;
using CMS.Domain.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebEnv.Util.Mailer.Settings;

namespace CMS.API.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("[controller]")]
    public class UserController : CustomControllerBase
    {
        private readonly SmtpSettings _smtpSettings;
        private readonly EmailSettings _emailSettings;
        private readonly OrganisationSettings _organisationSettings;

        public UserController(CMSContext cmsContext,
                              ILogger<UserController> logger,
                              IMapper mapper,
                              IOptions<SmtpSettings> smtpSettings,
                              IOptions<EmailSettings> emailSettings,
                              IOptions<OrganisationSettings> organisationSettings) : base(cmsContext, logger, mapper)
        {
            _smtpSettings = smtpSettings.Value;
            _emailSettings = emailSettings.Value;
            _organisationSettings = organisationSettings.Value;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<IEnumerable<UserDownloadModel>>> GetUsers(int page, int pageSize)
        {
            try
            {
                if (await IsUserValidAsync())
                {
                    var userIsAdmin = await UserModel.CheckUserIsAdminByIdAsync(ExtractUserIdFromToken(), RepositoryManager.UserRepository);

                    if (userIsAdmin)
                    {
                        var users = await UserModel.GetUsersPageAsync(RepositoryManager.UserRepository, page, pageSize);

                        return Ok(MapEntitiesToDownloadModels<VGetUser, UserDownloadModel>(users));
                    }
                    else
                    {
                        return Forbid();
                    }
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception err)
            {
                LogException(err);

                return Problem();
            }
        }

        [HttpGet("GetById")]
        public async Task<ActionResult<UserDownloadModel>> GetUserById(Guid userId)
        {
            try
            {
                if (await IsUserValidAsync())
                {
                    var userIsAdmin = await UserModel.CheckUserIsAdminByIdAsync(ExtractUserIdFromToken(), RepositoryManager.UserRepository);

                    if (userIsAdmin)
                    {
                        var user = await UserModel.GetUserByIdAsync(userId, RepositoryManager.UserRepository);

                        return Ok(MapEntityToDownloadModel<User, UserDownloadModel>(user));
                    }
                    else
                    {
                        return Forbid();
                    }
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (Exception err)
            {
                LogException(err);

                return Problem();
            }
        }

        [HttpPost("CreateUser")]
        public async Task<ActionResult<UserDownloadModel>> CreateUser(UserUploadModel user)
        {
            try
            {
                if (await IsUserValidAsync())
                {
                    var userIsAdmin = await UserModel.CheckUserIsAdminByIdAsync(ExtractUserIdFromToken(), RepositoryManager.UserRepository);

                    if (userIsAdmin)
                    {
                        var newUser = MapUploadModelToEntity<User>(user);

                        var requesterId = ExtractUserIdFromToken();

                        if (user.IsAdmin)
                        {
                            if (user.AdminPassword != null)
                            {
                                var requesterIsValidAdmin = await UserModel.CheckUserCredentialsValidAsync(
                                    RepositoryManager.UserRepository,
                                    requesterId,
                                    user.AdminPassword);

                                if (!requesterIsValidAdmin)
                                {
                                    return Forbid();
                                }
                            }
                            else
                            {
                                return Forbid();
                            }
                        }

                        newUser.CreatedBy = requesterId;
                        newUser.CreatedOn = DateTime.Now;
                        newUser.LastUpdatedBy = requesterId;
                        newUser.LastUpdatedOn = DateTime.Now;

                        await UserModel.CreateNewUserAsync(
                            newUser,
                            ExtractRequesterAddress(),
                            RepositoryManager,
                            _smtpSettings,
                            _emailSettings);

                        return Ok(MapEntityToDownloadModel<User, UserDownloadModel>(newUser));
                    }
                    else
                    {
                        return Forbid();
                    }
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (EmailAlreadyRegisteredException err)
            {
                return BadRequest(new EmailAlreadyRegisteredException(err.ErrorMessage, err.ErrorData));
            }
            catch (Exception err)
            {
                LogException(err);

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
                LogException(err);

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
                LogException(err);

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
                LogException(err);

                return Problem();
            }
        }

        [HttpPost("SetPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> SetPassword(PasswordResetUploadModel passwordResetUploadModel)
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
                LogException(err);

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
                LogException(err);

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
                LogException(err);

                return Problem();
            }
        }
    }
}
