using AutoMapper;
using CMS.API.Infrastructure.Encryption;
using CMS.API.Models.User;
using CMS.API.UploadModels;
using CMS.Domain.Entities;
using CMS.Domain.Enums;
using CMS.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CMS.API.Controllers
{
    public class CustomControllerBase : ControllerBase
    {
        public IRepositoryManager RepositoryManager { get; private set; }
        private readonly IMapper _mapper;

        public CustomControllerBase(CMSContext cmsContext,
                                    IMapper mapper)
        {
            RepositoryManager = new RepositoryManager(cmsContext);
            _mapper = mapper;
        }

        protected string ExtractRequesterAddress()
        {
            return Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }

        protected Guid ExtractUserIdFromToken()
        {
            var userIdString = User.FindFirst(ClaimTypes.Name)?.Value;
            var userId = Guid.Parse(userIdString);

            return userId;
        }

        protected string ExtractUserSecretFromToken()
        {
            var userSecret = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            return userSecret;
        }

        protected async Task<bool> IsUserValidAsync()
        {
            var userIsValid = await UserModel.CheckUserExistsByIdAsync(
                ExtractUserIdFromToken(), RepositoryManager.UserRepository);

            if (userIsValid)
            {
                var user = await UserModel.GetUserByIdAsync(
                    ExtractUserIdFromToken(), RepositoryManager.UserRepository);

                if (user.UserSecret != null)
                {
                    var decryptedUserSecret = DecryptionService.DecryptString(user.UserSecret);
                    var decryptedExtractedUserSecret = DecryptionService.DecryptString(ExtractUserSecretFromToken());

                    if (decryptedUserSecret == decryptedExtractedUserSecret)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return userIsValid;
            }
        }

        protected async Task<AuditLog> LogAction(UserActionCategory actionCategory, UserAction action, Guid userId, DateTime occurredOn)
        {
            var auditLog = new AuditLog
            {
                ActionCategory = (short)actionCategory,
                Action = (short)action,
                UserId = userId,
                UserAddress = ExtractRequesterAddress(),
                OccurredOn = occurredOn
            };

            return await RepositoryManager.AuditLogRepository.AddAsync(auditLog);
        }

        protected TDownloadModel MapEntityToDownloadModel<TEntity, TDownloadModel>(TEntity entity)
        {
            return _mapper.Map<TDownloadModel>(entity);
        }

        protected List<TDownloadModel> MapEntitiesToDownloadModels<TEntity, TDownloadModel>(IEnumerable<TEntity> entity)
        {
            return _mapper.Map<IEnumerable<TEntity>, List<TDownloadModel>>(entity);
        }

        protected TEntity MapUploadModelToEntity<TEntity>(IUploadModel uploadModel)
        {
            return _mapper.Map<TEntity>(uploadModel);
        }

        protected async Task<PasswordReset> GeneratePasswordSetLink(Guid userId, string requesterAddress)
        {
            var passwordReset = new PasswordReset
            {
                Identifier = GenerateResetIdentifier(),
                UserId = EncryptionService.EncryptUserId(userId),
                ExpiryDate = DateTime.Now.AddDays(15),
                RequesterAddress = requesterAddress,
                Active = true
            };

            await RepositoryManager.PasswordResetRepository.AddAsync(passwordReset);

            return passwordReset;
        }

        private string GenerateResetIdentifier()
        {
            var random = new Random();

            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var resetIdentifier = new string(
                Enumerable.Repeat(chars, 32)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());

            return resetIdentifier;
        }
    }
}
