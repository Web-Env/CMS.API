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
            var content = (await contentRepository.FindAsync(c => c.Path == contentPath)).FirstOrDefault();

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

            var contentId = content.Id.ToString().ToLower();
            BlobServiceClient blobServiceClient = new BlobServiceClient(azureStorageConnectionString);
            BlobContainerClient containerClient = await blobServiceClient.CreateBlobContainerAsync(contentId);
            BlobClient blobClient = containerClient.GetBlobClient(contentId);

            var contentStream = Encoding.UTF8.GetBytes(contentUploadModel.Content);

            using (var memStream = new MemoryStream(contentStream))
            {
                await blobClient.UploadAsync(memStream, true);
            }

            return content;
        }

        public static async Task DeleteContentAsync(
            Guid contentId,
            IContentRepository contentRepository,
            string azureStorageConnectionString)
        {
            var contentIdString = contentId.ToString().ToLower();
            BlobServiceClient blobServiceClient = new BlobServiceClient(azureStorageConnectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(contentIdString);

            await containerClient.DeleteAsync();

            var content = await contentRepository.GetByIdAsync(contentId);
            await contentRepository.RemoveAsync(content);
        }
    }
}
