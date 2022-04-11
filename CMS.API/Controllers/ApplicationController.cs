using AutoMapper;
using CMS.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CMS.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApplicationController : CustomControllerBase
    {
        public ApplicationController(CMSContext cmsContext,
                                     IMapper mapper) : base(cmsContext, mapper) { }

        [HttpGet("Version")]
        public ActionResult<string> GetVersion()
        {
            var version = typeof(Startup).Assembly.GetName().Version.ToString();

            return Ok(version);
        }

        [HttpGet("/Ip")]
        public ActionResult<string> Get()
        {
            return Ok(ExtractRequesterAddress());
        }
    }
}
