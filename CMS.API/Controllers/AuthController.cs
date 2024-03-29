﻿using AutoMapper;
using CMS.API.Infrastructure.Exceptions;
using CMS.API.Services.Authentication;
using CMS.API.UploadModels.Auth;
using CMS.Domain.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace CMS.API.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("[controller]")]
    public class AuthController : CustomControllerBase
    {
        private readonly AuthenticationService _authenticationService;

        public AuthController(CMSContext cmsContext,
                              ILogger<AuthController> logger,
                              IMapper mapper,
                              AuthenticationService authenticationService) : base(cmsContext, logger, mapper)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost("Auth")]
        [AllowAnonymous]
        public async Task<IActionResult> Authenticate(AuthenticationRequest authenticationRequest)
        {
            try
            {
                var response = await _authenticationService.AuthenticateAsync(authenticationRequest, RepositoryManager.UserRepository);

                return Ok(response);
            }
            catch (AuthenticationException authException)
            {
                return BadRequest(authException.ErrorMessage);
            }
            catch (Exception err)
            {
                LogException(err);

                return Problem();
            }
        }

        [HttpGet("Validate")]
        public IActionResult Validate()
        {
            return Ok();
        }
    }
}
