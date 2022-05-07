using AutoMapper;
using CMS.API.DownloadModels.Content;
using CMS.API.Models.Content;
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
    public class SidebarController : CustomControllerBase
    {
        public SidebarController(CMSContext cmsContext,
                                 ILogger<SidebarController> logger,
                                 IMapper mapper) : base(cmsContext, logger, mapper) { }

        [HttpGet("GetSidebarButtons")]
        public async Task<ActionResult<IEnumerable<SidebarButtonDownloadModel>>> GetSidebarButtons()
        {
            try
            {
                if (await IsUserValidAsync())
                {
                    var sidebarButtons = await SidebarModel.GetSidebarButtonsAsync(RepositoryManager, ExtractUserIdFromToken());

                    return Ok(sidebarButtons);
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