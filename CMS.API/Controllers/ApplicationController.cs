using CMS.API.DownloadModels.Application;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CMS.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class ApplicationController : Controller
    {
        [HttpGet("/ip")]
        [AllowAnonymous]
        public IActionResult Get()
        {
            var userAddress = Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            var ipDownloadModel = new IpDownloadModel
            {
                Ip = userAddress
            };

            return Ok(ipDownloadModel);
        }
    }
}
