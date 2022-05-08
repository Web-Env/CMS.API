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
                    Path = "",
                    Url = ""
                };
            }

            var contentId = content != null ? content.Id.ToString().ToLowerInvariant() : Guid.Empty.ToString().ToLowerInvariant();
            BlobServiceClient blobServiceClient = new BlobServiceClient(azureStorageConnectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(contentId);
            BlobClient blobClient = containerClient.GetBlobClient(contentId);

            string contentString;
            using (var memStream = new MemoryStream())
            {
                await blobClient.DownloadToAsync(memStream);

                contentString = Encoding.UTF8.GetString(memStream.ToArray());
            }

            var contentDownloadModel = mapper.Map<Domain.Entities.Content, ContentDownloadModel>(content);
            contentDownloadModel.Content = contentString;

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

            await contentRepository.AddAsync(content);

            await UploadContentBlobAsync(content.Id, contentUploadModel, azureStorageConnectionString).ConfigureAwait(false);

            return content;
        }

        private static async Task UploadContentBlobAsync(
            Guid contentId,
            ContentUploadModel contentUploadModel,
            string azureStorageConnectionString)
        {
            var contentIdString = contentId.ToString().ToLowerInvariant();
            BlobServiceClient blobServiceClient = new BlobServiceClient(azureStorageConnectionString);
            BlobContainerClient containerClient = await blobServiceClient.CreateBlobContainerAsync(contentIdString);
            BlobClient blobClient = containerClient.GetBlobClient(contentIdString);

            var contentStream = Encoding.UTF8.GetBytes(contentUploadModel.Content);

            using (var memStream = new MemoryStream(contentStream))
            {
                await blobClient.UploadAsync(memStream, true);
            }
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

            await DeleteContentBlobAsync(content.Id, azureStorageConnectionString).ConfigureAwait(false);
            await UploadContentBlobToContainerAsync(content.Id, contentUploadModel, azureStorageConnectionString).ConfigureAwait(false);

            return content;
        }

        private static async Task UploadContentBlobToContainerAsync(
            Guid contentId,
            ContentUploadModel contentUploadModel,
            string azureStorageConnectionString)
        {
            var contentIdString = contentId.ToString().ToLowerInvariant();
            BlobServiceClient blobServiceClient = new BlobServiceClient(azureStorageConnectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(contentIdString);
            BlobClient blobClient = containerClient.GetBlobClient(contentIdString);

            var contentStream = Encoding.UTF8.GetBytes(contentUploadModel.Content);

            using (var memStream = new MemoryStream(contentStream))
            {
                await blobClient.UploadAsync(memStream, true);
            }
        }

        private static async Task DeleteContentBlobAsync(Guid contentId, string azureStorageConnectionString)
        {
            var contentIdString = contentId.ToString().ToLowerInvariant();
            BlobServiceClient blobServiceClient = new BlobServiceClient(azureStorageConnectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(contentIdString);

            await containerClient.GetBlobClient(contentIdString).DeleteAsync();
        }

        public static async Task DeleteContentAsync(
            Guid contentId,
            IRepositoryManager repositoryManager,
            string azureStorageConnectionString)
        {
            await DeleteContentBlobContainerAsync(contentId, azureStorageConnectionString).ConfigureAwait(false);

            var content = await repositoryManager.ContentRepository.GetByIdAsync(contentId);
            var contentTimeTrackings = await repositoryManager.ContentTimeTrackingRepository.GetByContentIdAsync(contentId);

            await repositoryManager.ContentTimeTrackingRepository.RemoveRangeAsync(contentTimeTrackings);
            await repositoryManager.ContentRepository.RemoveAsync(content);
        }

        private static async Task DeleteContentBlobContainerAsync(Guid contentId, string azureStorageConnectionString)
        {
            var contentIdString = contentId.ToString().ToLowerInvariant();
            BlobServiceClient blobServiceClient = new BlobServiceClient(azureStorageConnectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(contentIdString);

            await containerClient.DeleteAsync();
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
