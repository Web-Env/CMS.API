using AutoMapper;
using CMS.API.DownloadModels.Content;
using CMS.API.Models.Content;
using CMS.API.Models.User;
using CMS.API.UploadModels.Content;
using CMS.Domain.Entities;
using CMS.Domain.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        public ContentController(IRepositoryManager repositoryManager, IMapper mapper) : base(repositoryManager, mapper) { }

        [HttpGet("GetAll")]
        public async Task<ActionResult<IEnumerable<ContentDownloadModel>>> GetSections(int page, int pageSize)
        {
            try
            {
                if (await IsUserValidAsync())
                {
                    var userIsAdmin = await UserModel.CheckUserIsAdminByIdAsync(ExtractUserIdFromToken(), RepositoryManager.UserRepository);

                    if (userIsAdmin)
                    {
                        var sections = await ContentModel.GetContentPageAsync(page, pageSize, RepositoryManager.ContentRepository);

                        return Ok(MapEntitiesToDownloadModels<Content, ContentDownloadModel>(sections));
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
                //LogException(err);

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
                        RepositoryManager.ContentRepository);

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
                return Problem();
            }
        }
    }
}
