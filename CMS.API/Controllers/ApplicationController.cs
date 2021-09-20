using AutoMapper;
using CMS.Domain.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace CMS.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApplicationController : CustomControllerBase
    {
        public ApplicationController(IRepositoryManager repositoryManager,
                                     IMapper mapper) : base(repositoryManager, mapper) { }

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
