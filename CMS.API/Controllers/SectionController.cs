using AutoMapper;
using CMS.API.DownloadModels.Content;
using CMS.API.Infrastructure.Exceptions;
using CMS.API.Models.Content;
using CMS.API.Models.User;
using CMS.API.UploadModels.Content;
using CMS.Domain.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CMS.API.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("[controller]")]
    public class SectionController : CustomControllerBase
    {
        public SectionController(CMSContext cmsContext,
                                 ILogger<SectionController> logger,
                                 IMapper mapper) : base(cmsContext, logger, mapper) { }

        [HttpGet("GetAll")]
        public async Task<ActionResult<IEnumerable<SectionDownloadModel>>> GetSections(int page, int pageSize)
        {
            try
            {
                if (await IsUserValidAsync())
                {
                    var userIsAdmin = await UserModel.CheckUserIsAdminByIdAsync(ExtractUserIdFromToken(), RepositoryManager.UserRepository);

                    if (userIsAdmin)
                    {
                        var sections = await SectionModel.GetSectionsPageAsync(page, pageSize, RepositoryManager.SectionRepository);

                        return Ok(MapEntitiesToDownloadModels<Section, SectionDownloadModel>(sections));
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

        [HttpPost("Add")]
        public async Task<ActionResult<SectionDownloadModel>> AddSection(SectionUploadModel sectionUploadModel)
        {
            try
            {
                if (await IsUserValidAsync())
                {
                    var userIsAdmin = await UserModel.CheckUserIsAdminByIdAsync(ExtractUserIdFromToken(), RepositoryManager.UserRepository);

                    if (userIsAdmin)
                    {
                        var section = await SectionModel.AddSectionAsync(
                            sectionUploadModel,
                            ExtractUserIdFromToken(),
                            RepositoryManager.SectionRepository);

                        return Ok(MapEntityToDownloadModel<Section, SectionDownloadModel>(section));
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
        public async Task<ActionResult<SectionDownloadModel>> UpdateSection(SectionUploadModel sectionUploadModel)
        {
            try
            {
                if (await IsUserValidAsync())
                {
                    var userIsAdmin = await UserModel.CheckUserIsAdminByIdAsync(ExtractUserIdFromToken(), RepositoryManager.UserRepository);

                    if (userIsAdmin)
                    {
                        var section = await SectionModel.UpdateSectionAsync(
                            sectionUploadModel,
                            ExtractUserIdFromToken(),
                            RepositoryManager.SectionRepository);

                        return Ok(MapEntityToDownloadModel<Section, SectionDownloadModel>(section));
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
        public async Task<ActionResult> DeleteSection(Guid sectionId)
        {
            try
            {
                if (await IsUserValidAsync())
                {
                    var userIsAdmin = await UserModel.CheckUserIsAdminByIdAsync(ExtractUserIdFromToken(), RepositoryManager.UserRepository);

                    if (userIsAdmin)
                    {
                        await SectionModel.DeleteSectionAsync(
                            sectionId,
                            RepositoryManager.SectionRepository
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
            catch (SectionHasContentException err)
            {
                return BadRequest(new SectionHasContentException(err.Message, err.ErrorData));
            }
            catch (Exception err)
            {
                LogException(err);

                return Problem();
            }
        }
    }
}
