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

                        await UserModel.CreateNewUserAsync(
                            newUser,
                            ExtractRequesterAddress(),
                            requesterId,
                            RepositoryManager,
                            _smtpSettings,
                            _emailSettings);

                        var createdByUser = await UserModel.GetUserByIdAsync(ExtractUserIdFromToken(), RepositoryManager.UserRepository);
                        var mappedNewUser = MapEntityToDownloadModel<User, UserDownloadModel>(newUser);
                        mappedNewUser.CreatedBy = $"{createdByUser.FirstName} {createdByUser.LastName}";

                        return Ok(mappedNewUser);
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

        [HttpPut("UpdateUser")]
        public async Task<ActionResult<UserDownloadModel>> UpdateUser(UserUploadModel user)
        {
            try
            {
                if (await IsUserValidAsync())
                {
                    var userIsAdmin = await UserModel.CheckUserIsAdminByIdAsync(ExtractUserIdFromToken(), RepositoryManager.UserRepository);

                    if (userIsAdmin)
                    {
                        var requesterId = ExtractUserIdFromToken();

                        var updatedUser = await UserModel.UpdateUserAsync(
                            user,
                            requesterId,
                            RepositoryManager.UserRepository);

                        var createdByUser = await UserModel.GetUserByIdAsync(ExtractUserIdFromToken(), RepositoryManager.UserRepository);
                        var mappedNewUser = MapEntityToDownloadModel<User, UserDownloadModel>(updatedUser);
                        mappedNewUser.CreatedBy = $"{createdByUser.FirstName} {createdByUser.LastName}";

                        return Ok(mappedNewUser);
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
            catch (InvalidTokenException err)
            {
                return Forbid();
            }
            catch (Exception err)
            {
                LogException(err);

                return Problem();
            }
        }

        [HttpDelete("DeleteUser")]
        public async Task<ActionResult> DeleteUser(Guid userId)
        {
            try
            {
                if (await IsUserValidAsync())
                {
                    var userIsAdmin = await UserModel.CheckUserIsAdminByIdAsync(ExtractUserIdFromToken(), RepositoryManager.UserRepository);

                    if (userIsAdmin)
                    {
                        var requesterId = ExtractUserIdFromToken();

                        await UserModel.DeleteUserAsync(
                            userId,
                            ExtractUserIdFromToken(),
                            RepositoryManager.UserRepository
                        );

                        return Ok();
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
