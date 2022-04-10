using CMS.API.UploadModels.Content;
using CMS.Domain.Entities;
using CMS.Domain.Repositories.Content.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.API.Models.Content
{
    public static class ContentModel
    {
        public static async Task<IEnumerable<Domain.Entities.Content>> GetContentPageAsync(int page, int pageSize, IContentRepository contentRepository)
        {
            var sections = await contentRepository.GetPageAsync(page, pageSize);
            return sections;
        }

        public static async Task<Domain.Entities.Content> AddContentAsync(ContentUploadModel contentUploadModel, Guid userId, IContentRepository contentRepository)
        {
            var content = new Domain.Entities.Content
            {
                Title = contentUploadModel.Title,
                Path = contentUploadModel.Path,
                Active = contentUploadModel.Active,
                Url = "",
                Views = 0,
                CreatedOn = DateTime.Now,
                CreatedBy = userId,
                LastUpdatedOn = DateTime.Now,
                LastUpdatedBy = userId
            };

            if (contentUploadModel.SectionId != null)
            {
                content.SectionId = contentUploadModel.SectionId;
            }

            return await contentRepository.AddAsync(content);
        }
    }
}
