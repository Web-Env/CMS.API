using AutoMapper;
using CMS.API.DownloadModels.Content;
using CMS.API.UploadModels.Content;
using CMS.Domain.Entities;
using CMS.Domain.Repositories;
using CMS.Domain.Repositories.Content.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CMS.API.Models.Content
{
    public static class AnnouncementModel
    {
        public static async Task<IEnumerable<Domain.Entities.Content>> GetContentPageAsync(int page, int pageSize, IContentRepository contentRepository)
        {
            var contents = await contentRepository.GetPageAsync(page, pageSize);
            return contents;
        }

        public static async Task<AnnouncementDownloadModel> GetAnnouncementAsync(
            string announcementPath,
            IAnnouncementRepository announcementRepository,
            string azureStorageConnectionString,
            IMapper mapper)
        {
            Announcement announcement = await announcementRepository.GetByPathAsync(announcementPath);

            var announcementDownloadModel = mapper.Map<Announcement, AnnouncementDownloadModel>(announcement);

            announcementDownloadModel.Content = await AzureBlobModel.GetContentStringByIdAsync(
                announcement.Id, 
                azureStorageConnectionString);

            if (announcement != null && announcement.Id != Guid.Empty)
            {
                announcement.Views++;
                announcement.CreatedByNavigation = null;

                await announcementRepository.UpdateAsync(announcement);
            }

            return announcementDownloadModel;
        }

        public static async Task<Domain.Entities.Content> AddContentAsync(
            ContentUploadModel contentUploadModel,
            Guid userId,
            IContentRepository contentRepository,
            string azureStorageConnectionString)
        {
            var content = new Domain.Entities.Content
            {
                Title = contentUploadModel.Title,
                Path = contentUploadModel.Path,
                Active = contentUploadModel.Active,
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

            await contentRepository.AddAsync(content);

            await AzureBlobModel.UploadContentBlobAsync(
                content.Id, 
                contentUploadModel, 
                azureStorageConnectionString).ConfigureAwait(false);

            return content;
        }

        public static async Task<Domain.Entities.Content> UpdateContentAsync(
            ContentUploadModel contentUploadModel,
            Guid userId,
            IContentRepository contentRepository,
            string azureStorageConnectionString)
        {
            Domain.Entities.Content content = null;
            if (contentUploadModel.Id != Guid.Empty)
            {
                content = await contentRepository.GetByIdAsync(contentUploadModel.Id.Value);

                content.Title = contentUploadModel.Title;
                content.Path = contentUploadModel.Path;
                content.SectionId = contentUploadModel.SectionId;
                content.LastUpdatedOn = DateTime.Now;
                content.LastUpdatedBy = userId;

                await contentRepository.UpdateAsync(content);
            }
            else
            {
                content = new Domain.Entities.Content
                {
                    Id = contentUploadModel.Id.Value
                };
            }

            await AzureBlobModel.DeleteContentBlobAsync(content.Id, azureStorageConnectionString).ConfigureAwait(false);
            await AzureBlobModel.UploadContentBlobToContainerAsync(
                content.Id, 
                contentUploadModel, 
                azureStorageConnectionString).ConfigureAwait(false);

            return content;
        }

        public static async Task DeleteContentAsync(
            Guid contentId,
            IRepositoryManager repositoryManager,
            string azureStorageConnectionString)
        {
            await AzureBlobModel.DeleteContentBlobContainerAsync(contentId, azureStorageConnectionString).ConfigureAwait(false);

            var content = await repositoryManager.ContentRepository.GetByIdAsync(contentId);
            var contentTimeTrackings = await repositoryManager.ContentTimeTrackingRepository.GetByContentIdAsync(contentId);

            await repositoryManager.ContentTimeTrackingRepository.RemoveRangeAsync(contentTimeTrackings);
            await repositoryManager.ContentRepository.RemoveAsync(content);
        }
    }
}
