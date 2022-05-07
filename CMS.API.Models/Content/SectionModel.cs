using CMS.API.Infrastructure.Exceptions;
using CMS.API.UploadModels.Content;
using CMS.Domain.Entities;
using CMS.Domain.Repositories.Content.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public static async Task<Section> AddSectionAsync(
            SectionUploadModel sectionUploadModel,
            Guid userId,
            ISectionRepository sectionRepository)
        {
            var section = new Section
            {
                Title = sectionUploadModel.Title,
                Path = sectionUploadModel.Path,
                Active = sectionUploadModel.Active,
                CreatedOn = DateTime.Now,
                CreatedBy = userId,
                LastUpdatedOn = DateTime.Now,
                LastUpdatedBy = userId
            };

            return await sectionRepository.AddAsync(section);
        }

        public static async Task<Section> UpdateSectionAsync(
            SectionUploadModel sectionUploadModel,
            Guid userId,
            ISectionRepository sectionRepository)
        {
            var section = await sectionRepository.GetByIdAsync(sectionUploadModel.Id);

            section.Title = sectionUploadModel.Title;
            section.Path = sectionUploadModel.Path;
            section.LastUpdatedOn = DateTime.Now;
            section.LastUpdatedBy = userId;

            return await sectionRepository.UpdateAsync(section);
        }

        public static async Task DeleteSectionAsync(Guid sectionId, ISectionRepository sectionRepository)
        {
            var section = await sectionRepository.GetByIdAsync(sectionId);

            if (section.Contents.Any())
            {
                throw new SectionHasContentException(
                    "This Section has Content associated with it",
                    "This Section has Content associated with it"
                );
            }

            await sectionRepository.RemoveByIdAsync(sectionId);
        }
    }
}
