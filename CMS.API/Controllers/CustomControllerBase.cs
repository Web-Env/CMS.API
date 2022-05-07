using AutoMapper;
using CMS.API.Infrastructure.Encryption;
using CMS.API.Models.User;
using CMS.API.UploadModels;
using CMS.Domain.Entities;
using CMS.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
        protected readonly ILogger Logger;
        private readonly IMapper _mapper;

        public CustomControllerBase(CMSContext cmsContext,
                                    ILogger<CustomControllerBase> logger,
                                    IMapper mapper)
        {
            RepositoryManager = new RepositoryManager(cmsContext);
            Logger = logger;
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
            var userTokenIsValid = await UserModel.CheckUserExistsByIdAsync(
                ExtractUserIdFromToken(), RepositoryManager.UserRepository);

            if (userTokenIsValid)
            {
                var user = await UserModel.GetUserByIdAsync(
                    ExtractUserIdFromToken(), RepositoryManager.UserRepository);

                if (user.ExpiresOn < DateTime.Now || user.Deleted)
                {
                    return false;
                }

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
                return userTokenIsValid;
            }
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

        protected void LogException(Exception exception)
        {
            Logger.LogError(exception, exception.Message);
        }
    }
}
