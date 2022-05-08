using AutoMapper;
using CMS.API.DownloadModels.Content;
using CMS.API.Infrastructure.Exceptions;
using CMS.API.Infrastructure.Settings;
using CMS.API.Models.Content;
using CMS.API.Models.User;
using CMS.API.UploadModels.Content;
using CMS.Domain.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CMS.API.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("[controller]")]
    public class ContentController : CustomControllerBase
    {
        private readonly AzureStorageSettings _azureStorageSettings;
        private readonly IMapper _mapper;

        public ContentController(
            CMSContext cmsContext,
            ILogger<ContentController> logger,
            IMapper mapper,
            IOptions<AzureStorageSettings> azureStorageSettings) : base(cmsContext, logger, mapper)
        {
            _azureStorageSettings = azureStorageSettings.Value;
            _mapper = mapper;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<IEnumerable<ContentDownloadModel>>> GetContents(int page, int pageSize)
        {
            try
            {
                if (await IsUserValidAsync())
                {
                    var userIsAdmin = await UserModel.CheckUserIsAdminByIdAsync(ExtractUserIdFromToken(), RepositoryManager.UserRepository);

                    if (userIsAdmin)
                    {
                        var contents = await ContentModel.GetContentPageAsync(page, pageSize, RepositoryManager.ContentRepository);

                        return Ok(MapEntitiesToDownloadModels<Content, ContentDownloadModel>(contents));
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

        [HttpGet("Get")]
        public async Task<ActionResult<ContentDownloadModel>> GetContent(string contentPath)
        {
            try
            {
                if (await IsUserValidAsync())
                {
                    var content = await ContentModel.GetContentAsync(
                        contentPath,
                        RepositoryManager.ContentRepository,
                        _azureStorageSettings.ConnectionString,
                        _mapper);

                    return Ok(content);
                }
                else
                {
                    return Unauthorized();
                }
            }
            catch (NotFoundException _)
            {
                return NotFound();
            }
            catch (Exception err)
            {
                LogException(err);

                return Problem();
            }
        }

        [HttpPost("Add")]
        public async Task<ActionResult<ContentDownloadModel>> AddContent(ContentUploadModel contentUploadModel)
        {
            try
            {
                if (await IsUserValidAsync())
                {
                    var userIsAdmin = await UserModel.CheckUserIsAdminByIdAsync(ExtractUserIdFromToken(), RepositoryManager.UserRepository);

                    if (userIsAdmin)
                    {
                        var content = await ContentModel.AddContentAsync(
                            contentUploadModel,
                            ExtractUserIdFromToken(),
                            RepositoryManager.ContentRepository,
                            _azureStorageSettings.ConnectionString);

                        return Ok(MapEntityToDownloadModel<Content, ContentDownloadModel>(content));
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

        [HttpPut("Update")]
        public async Task<ActionResult<ContentDownloadModel>> EditContent(ContentUploadModel contentUploadModel)
        {
            try
            {
                if (await IsUserValidAsync())
                {
                    var userIsAdmin = await UserModel.CheckUserIsAdminByIdAsync(ExtractUserIdFromToken(), RepositoryManager.UserRepository);

                    if (userIsAdmin)
                    {
                        var content = await ContentModel.UpdateContentAsync(
                            contentUploadModel,
                            ExtractUserIdFromToken(),
                            RepositoryManager.ContentRepository,
                            _azureStorageSettings.ConnectionString);

                        return Ok(MapEntityToDownloadModel<Content, ContentDownloadModel>(content));
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

        [HttpDelete("Delete")]
        public async Task<ActionResult> DeleteContent(Guid contentId)
        {
            try
            {
                if (await IsUserValidAsync())
                {
                    var userIsAdmin = await UserModel.CheckUserIsAdminByIdAsync(ExtractUserIdFromToken(), RepositoryManager.UserRepository);

                    if (userIsAdmin)
                    {
                        if (contentId != Guid.Empty)
                        {
                            await ContentModel.DeleteContentAsync(
                                contentId,
                                RepositoryManager,
                                _azureStorageSettings.ConnectionString);

                            return Ok();
                        }
                        else
                        {
                            return Forbid();
                        }
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

        [HttpGet("ContentTimeTracking/GetAllByUserId")]
        public async Task<ActionResult> TrackUserTime(Guid userId)
        {
            try
            {
                if (await IsUserValidAsync())
                {
                    var userIsAdmin = await UserModel.CheckUserIsAdminByIdAsync(ExtractUserIdFromToken(), RepositoryManager.UserRepository);

                    if (userIsAdmin)
                    {
                        var contentTimeTrackings = await ContentModel.GetUserTimeTrackingAsync(
                            userId,
                            RepositoryManager.ContentTimeTrackingRepository
                        );

                        return Ok(
                            MapEntitiesToDownloadModels<ContentTimeTracking, ContentTimeTrackingDownloadModel>(
                                contentTimeTrackings
                            )
                        );
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

        [HttpPost("ContentTimeTracking/Record")]
        public async Task<ActionResult> TrackUserTime(Guid contentId, int interval)
        {
            try
            {
                if (await IsUserValidAsync())
                {
                    await ContentModel.TrackUserTime(
                        contentId,
                        ExtractUserIdFromToken(),
                        interval,
                        RepositoryManager
                    );

                    return Ok();
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
    }
}
