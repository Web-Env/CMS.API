using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CMS.API.Controllers
{
    [ApiController]
    [Authorize]
    [Route("[controller]")]
    public class ApplicationController : Controller
    {
        [HttpGet("/Ip")]
        [AllowAnonymous]
        public IActionResult Get()
        {
            var userAddress = Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            return Ok(userAddress);
        }
    }
}
