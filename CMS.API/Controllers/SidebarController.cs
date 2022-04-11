using AutoMapper;
using CMS.API.DownloadModels.Content;
using CMS.API.Models.Content;
using CMS.API.Models.User;
using CMS.Domain.Entities;
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
    public class SidebarController : CustomControllerBase
    {
        public SidebarController(CMSContext cmsContext, IMapper mapper) : base(cmsContext, mapper) { }

        [HttpGet("GetSidebarButtons")]
        public async Task<ActionResult<IEnumerable<SidebarButtonDownloadModel>>> GetSidebarButtons()
        {
            try
            {
                if (await IsUserValidAsync())
                {
                    var sidebarButtons = await SidebarModel.GetSidebarButtonsAsync(RepositoryManager);

                    var userIsAdmin = await UserModel.CheckUserIsAdminByIdAsync(ExtractUserIdFromToken(), RepositoryManager.UserRepository);

                    if (userIsAdmin)
                    {
                        sidebarButtons.Add(SidebarModel.GetAdminSidebarButtons());
                    }

                    return Ok(sidebarButtons);
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
    }
}