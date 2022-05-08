using Azure.Storage.Blobs;
using CMS.API.UploadModels.Content;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace CMS.API.Models.Content
{
    public static class AzureBlobModel
    {
        public static async Task<string> GetContentStringByIdAsync(Guid id, string azureStorageConnectionString)
        {
            var idString = id.ToString().ToLowerInvariant();
            BlobServiceClient blobServiceClient = new BlobServiceClient(azureStorageConnectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(idString);
            BlobClient blobClient = containerClient.GetBlobClient(idString);

            string contentString;
            using (var memStream = new MemoryStream())
            {
                await blobClient.DownloadToAsync(memStream);

                contentString = Encoding.UTF8.GetString(memStream.ToArray());

                return contentString;
            }
        }

        public static async Task UploadContentBlobAsync(
            Guid id,
            ContentUploadModel contentUploadModel,
            string azureStorageConnectionString)
        {
            var idString = id.ToString().ToLowerInvariant();
            BlobServiceClient blobServiceClient = new BlobServiceClient(azureStorageConnectionString);
            BlobContainerClient containerClient = await blobServiceClient.CreateBlobContainerAsync(idString);
            BlobClient blobClient = containerClient.GetBlobClient(idString);

            var contentStream = Encoding.UTF8.GetBytes(contentUploadModel.Content);

            using (var memStream = new MemoryStream(contentStream))
            {
                await blobClient.UploadAsync(memStream, true);
            }
        }

        public static async Task UploadContentBlobToContainerAsync(
            Guid id,
            ContentUploadModel contentUploadModel,
            string azureStorageConnectionString)
        {
            var idString = id.ToString().ToLowerInvariant();
            BlobServiceClient blobServiceClient = new BlobServiceClient(azureStorageConnectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(idString);
            BlobClient blobClient = containerClient.GetBlobClient(idString);

            var contentStream = Encoding.UTF8.GetBytes(contentUploadModel.Content);

            using (var memStream = new MemoryStream(contentStream))
            {
                await blobClient.UploadAsync(memStream, true);
            }
        }

        public static async Task DeleteContentBlobAsync(Guid id, string azureStorageConnectionString)
        {
            var idString = id.ToString().ToLowerInvariant();
            BlobServiceClient blobServiceClient = new BlobServiceClient(azureStorageConnectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(idString);

            await containerClient.GetBlobClient(idString).DeleteAsync();
        }

        public static async Task DeleteContentBlobContainerAsync(Guid id, string azureStorageConnectionString)
        {
            var idString = id.ToString().ToLowerInvariant();
            BlobServiceClient blobServiceClient = new BlobServiceClient(azureStorageConnectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(idString);

            await containerClient.DeleteAsync();
        }
    }
}
