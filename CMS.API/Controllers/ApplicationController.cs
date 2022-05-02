using AutoMapper;
using CMS.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CMS.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApplicationController : CustomControllerBase
    {
        public ApplicationController(CMSContext cmsContext,
                                     ILogger<ApplicationController> logger,
                                     IMapper mapper) : base(cmsContext, logger, mapper) { }

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
