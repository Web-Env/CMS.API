using AutoMapper;
using Azure.Storage.Blobs;
using CMS.API.DownloadModels.Content;
using CMS.API.UploadModels.Content;
using CMS.Domain.Entities;
using CMS.Domain.Repositories;
using CMS.Domain.Repositories.Content.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CMS.API.Models.Content
{
    public static class ContentModel
    {
        public static async Task<IEnumerable<Domain.Entities.Content>> GetContentPageAsync(int page, int pageSize, IContentRepository contentRepository)
        {
            var contents = await contentRepository.GetPageAsync(page, pageSize);
            return contents;
        }

        public static async Task<ContentDownloadModel> GetContentAsync(
            string contentPath,
            IContentRepository contentRepository,
            string azureStorageConnectionString,
            IMapper mapper)
        {
            Domain.Entities.Content content;
            if (!string.IsNullOrWhiteSpace(contentPath))
            {
                content = await contentRepository.GetByPathAsync(contentPath);
            }
            else
            {
                content = new Domain.Entities.Content
                {
                    Title = "Home",
                    Path = ""
                };
            }

            var contentDownloadModel = mapper.Map<Domain.Entities.Content, ContentDownloadModel>(content);

            var contentId = content != null ? content.Id : Guid.Empty;
            contentDownloadModel.Content = await AzureBlobModel.GetContentStringByIdAsync(
                contentId,
                azureStorageConnectionString);

            if (content != null && content.Id != Guid.Empty)
            {
                content.Views++;
                content.CreatedByNavigation = null;

                await contentRepository.UpdateAsync(content);
            }

            return contentDownloadModel;
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

        public static async Task<IEnumerable<ContentTimeTracking>> GetUserTimeTrackingAsync(
            Guid userId,
            IContentTimeTrackingRepository contentTimeTrackingRepository)
        {
            return await contentTimeTrackingRepository.GetByUserIdAsync(userId).ConfigureAwait(false);
        }

        public static async Task TrackUserTime(Guid contentId, Guid userId, int interval, IRepositoryManager repositoryManager)
        {
            var contentTimeTracking = await repositoryManager.ContentTimeTrackingRepository.GetByContentIdAndUserIdAsync(
                contentId,
                userId
            );

            if (contentTimeTracking == null)
            {
                contentTimeTracking = new ContentTimeTracking
                {
                    ContentId = contentId,
                    UserId = userId
                };
            }

            contentTimeTracking.TotalTime += interval;
            contentTimeTracking.LastSeen = DateTime.Now;

            if (contentTimeTracking.Id == Guid.Empty)
            {
                await repositoryManager.ContentTimeTrackingRepository.AddAsync(contentTimeTracking);
            }
            else
            {
                await repositoryManager.ContentTimeTrackingRepository.UpdateAsync(contentTimeTracking);
            }
        }
    }
}
