using AutoMapper;
using Azure.Storage.Blobs;
using CMS.API.DownloadModels.Content;
using CMS.API.UploadModels.Content;
using CMS.Domain.Entities;
using CMS.Domain.Repositories.Content.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            var content = await contentRepository.GetByPathAsync(contentPath);

            var contentId = content.Id.ToString().ToLower();
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

            await UploadContentBlobAsync(content.Id, contentUploadModel, azureStorageConnectionString);

            return content;
        }

        private static async Task UploadContentBlobAsync(
            Guid contentId, 
            ContentUploadModel contentUploadModel, 
            string azureStorageConnectionString)
        {
            var contentIdString = contentId.ToString().ToLower();
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
            var content = await contentRepository.GetByIdAsync(contentUploadModel.Id.Value);

            content.Title = contentUploadModel.Title;
            content.Path = contentUploadModel.Path;
            content.SectionId = contentUploadModel.SectionId;
            content.LastUpdatedOn = DateTime.Now;
            content.LastUpdatedBy = userId;

            await contentRepository.UpdateAsync(content);

            await DeleteContentBlobAsync(content.Id, azureStorageConnectionString);
            await UploadContentBlobToContainerAsync(content.Id, contentUploadModel, azureStorageConnectionString);

            return content;
        }

        private static async Task UploadContentBlobToContainerAsync(
            Guid contentId,
            ContentUploadModel contentUploadModel,
            string azureStorageConnectionString)
        {
            var contentIdString = contentId.ToString().ToLower();
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
            var contentIdString = contentId.ToString().ToLower();
            BlobServiceClient blobServiceClient = new BlobServiceClient(azureStorageConnectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(contentIdString);

            await containerClient.GetBlobClient(contentIdString).DeleteAsync();
        }

        public static async Task DeleteContentAsync(
            Guid contentId,
            IContentRepository contentRepository,
            string azureStorageConnectionString)
        {
            await DeleteContentBlobContainerAsync(contentId, azureStorageConnectionString);

            var content = await contentRepository.GetByIdAsync(contentId);
            await contentRepository.RemoveAsync(content);
        }

        private static async Task DeleteContentBlobContainerAsync(Guid contentId, string azureStorageConnectionString)
        {
            var contentIdString = contentId.ToString().ToLower();
            BlobServiceClient blobServiceClient = new BlobServiceClient(azureStorageConnectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(contentIdString);

            await containerClient.DeleteAsync(); 
        }
    }
}
