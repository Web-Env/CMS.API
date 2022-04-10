﻿using AutoMapper;
using CMS.API.DownloadModels.Content;
using CMS.API.Models.Content;
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
    public class SectionController : CustomControllerBase
    {
        public SectionController(IRepositoryManager repositoryManager, IMapper mapper) : base(repositoryManager, mapper) { }

        [HttpGet("GetAll")]
        public async Task<ActionResult<IEnumerable<SectionDownloadModel>>> GetSections(int page, int pageSize)
        {
            try
            {
                if (await IsUserValidAsync())
                {
                    var sections = await SectionModel.GetSectionsPageAsync(page, pageSize, RepositoryManager.SectionRepository);

                    return Ok(MapEntitiesToDownloadModels<Section, SectionDownloadModel>(sections));
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
        public async Task<ActionResult<SectionDownloadModel>> AddSection(SectionUploadModel sectionUploadModel)
        {
            try
            {
                var section = await SectionModel.AddSectionAsync(
                    sectionUploadModel,
                    ExtractUserIdFromToken(),
                    RepositoryManager.SectionRepository);

                return Ok(MapEntityToDownloadModel<Section, SectionDownloadModel>(section));
            }
            catch (Exception err)
            {
                return Problem();
            }
        }
    }
}
