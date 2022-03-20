using CMS.API.UploadModels.Content;
using CMS.Domain.Entities;
using CMS.Domain.Repositories.Content.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CMS.API.Models.Content
{
    public static class SectionModel
    {
        public static async Task<IEnumerable<Section>> GetSectionsPageAsync(int page, int pageSize, ISectionRepository sectionRepository)
        {
            var sections = await sectionRepository.GetPageAsync(page, pageSize);
            return sections;
        }

        public static async Task AddSectionAsync(SectionUploadModel sectionUploadModel, Guid userId, ISectionRepository sectionRepository)
        {
            var section = new Section
            {
                Title = sectionUploadModel.Title,
                Path = sectionUploadModel.Path,
                Active = sectionUploadModel.Active,
                CreatedOn = sectionUploadModel.CreatedOn,
                CreatedBy = userId,
                LastUpdatedOn = sectionUploadModel.LastUpdatedOn,
                LastUpdatedBy = userId
            };

            await sectionRepository.AddAsync(section);
        }
    }
}
