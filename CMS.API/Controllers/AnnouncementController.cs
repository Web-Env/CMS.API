using AutoMapper;
using CMS.API.DownloadModels.Content;
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
    public class AnnouncementController : CustomControllerBase
    {
        private readonly AzureStorageSettings _azureStorageSettings;
        private readonly IMapper _mapper;

        public AnnouncementController(
            CMSContext cmsContext,
            ILogger<AnnouncementController> logger,
            IMapper mapper,
            IOptions<AzureStorageSettings> azureStorageSettings) : base(cmsContext, logger, mapper)
        {
            _azureStorageSettings = azureStorageSettings.Value;
            _mapper = mapper;
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<IEnumerable<AnnouncementDownloadModel>>> GetAnnouncements(int page, int pageSize)
        {
            try
            {
                if (await IsUserValidAsync())
                {
                    var userIsAdmin = await UserModel.CheckUserIsAdminByIdAsync(ExtractUserIdFromToken(), RepositoryManager.UserRepository);

                    if (userIsAdmin)
                    {
                        var announcements = await AnnouncementModel.GetAnnouncementsPageAsync(page, pageSize, RepositoryManager.AnnouncementRepository);

                        return Ok(MapEntitiesToDownloadModels<Announcement, AnnouncementDownloadModel>(announcements));
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
        public async Task<ActionResult<ContentDownloadModel>> GetAnnouncement(string announcementPath)
        {
            try
            {
                if (await IsUserValidAsync())
                {
                    var announcement = await AnnouncementModel.GetAnnouncementAsync(
                        announcementPath,
                        RepositoryManager.AnnouncementRepository,
                        _azureStorageSettings.ConnectionString,
                        _mapper);

                    return Ok(announcement);
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

        [HttpPost("Add")]
        public async Task<ActionResult<AnnouncementDownloadModel>> AddAnnouncement(ContentUploadModel contentUploadModel)
        {
            try
            {
                if (await IsUserValidAsync())
                {
                    var userIsAdmin = await UserModel.CheckUserIsAdminByIdAsync(ExtractUserIdFromToken(), RepositoryManager.UserRepository);

                    if (userIsAdmin)
                    {
                        var announcement = await AnnouncementModel.AddAnnouncementAsync(
                            contentUploadModel,
                            ExtractUserIdFromToken(),
                            RepositoryManager.AnnouncementRepository,
                            _azureStorageSettings.ConnectionString);

                        return Ok(MapEntityToDownloadModel<Announcement, AnnouncementDownloadModel>(announcement));
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
        public async Task<ActionResult<AnnouncementDownloadModel>> EditAnnouncement(ContentUploadModel contentUploadModel)
        {
            try
            {
                if (await IsUserValidAsync())
                {
                    var userIsAdmin = await UserModel.CheckUserIsAdminByIdAsync(ExtractUserIdFromToken(), RepositoryManager.UserRepository);

                    if (userIsAdmin)
                    {
                        var announcement = await AnnouncementModel.UpdateAnnouncementAsync(
                            contentUploadModel,
                            ExtractUserIdFromToken(),
                            RepositoryManager.AnnouncementRepository,
                            _azureStorageSettings.ConnectionString);

                        return Ok(MapEntityToDownloadModel<Announcement, AnnouncementDownloadModel>(announcement));
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
        public async Task<ActionResult> DeleteAnnouncement(Guid announcementId)
        {
            try
            {
                if (await IsUserValidAsync())
                {
                    var userIsAdmin = await UserModel.CheckUserIsAdminByIdAsync(ExtractUserIdFromToken(), RepositoryManager.UserRepository);

                    if (userIsAdmin)
                    {
                        if (announcementId != Guid.Empty)
                        {
                            await AnnouncementModel.DeleteAnnouncementAsync(
                                announcementId,
                                RepositoryManager.AnnouncementRepository,
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
    }
}
