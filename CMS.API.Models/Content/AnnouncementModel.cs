using AutoMapper;
using CMS.API.DownloadModels.Content;
using CMS.API.Infrastructure.Exceptions;
using CMS.API.UploadModels.Content;
using CMS.Domain.Entities;
using CMS.Domain.Repositories.Content.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CMS.API.Models.Content
{
    public static class AnnouncementModel
    {
        public static async Task<IEnumerable<Announcement>> GetAnnouncementsPageAsync(int page, int pageSize, IAnnouncementRepository announcementRepository)
        {
            var announcements = await announcementRepository.GetPageAsync(page, pageSize);
            return announcements;
        }

        public static async Task<AnnouncementDownloadModel> GetAnnouncementAsync(
            string announcementPath,
            IAnnouncementRepository announcementRepository,
            string azureStorageConnectionString,
            IMapper mapper)
        {
            Announcement announcement = await announcementRepository.GetByPathAsync(announcementPath);
            
            if (announcement == null)
            {
                throw new NotFoundException("Announcement Not Found", "Announcement Not Found");
            }

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

        public static async Task<Announcement> AddAnnouncementAsync(
            ContentUploadModel contentUploadModel,
            Guid userId,
            IAnnouncementRepository announcementRepository,
            string azureStorageConnectionString)
        {
            var announcement = new Announcement
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

            await announcementRepository.AddAsync(announcement);

            await AzureBlobModel.UploadContentBlobAsync(
                announcement.Id, 
                contentUploadModel, 
                azureStorageConnectionString).ConfigureAwait(false);

            return announcement;
        }

        public static async Task<Announcement> UpdateAnnouncementAsync(
            ContentUploadModel contentUploadModel,
            Guid userId,
            IAnnouncementRepository announcementRepository,
            string azureStorageConnectionString)
        {
            Announcement announcement = await announcementRepository.GetByIdAsync(contentUploadModel.Id.Value);

            announcement.Title = contentUploadModel.Title;
            announcement.Path = contentUploadModel.Path;
            announcement.LastUpdatedOn = DateTime.Now;
            announcement.LastUpdatedBy = userId;

            await announcementRepository.UpdateAsync(announcement);

            await AzureBlobModel.DeleteContentBlobAsync(announcement.Id, azureStorageConnectionString).ConfigureAwait(false);
            await AzureBlobModel.UploadContentBlobToContainerAsync(
                announcement.Id, 
                contentUploadModel, 
                azureStorageConnectionString).ConfigureAwait(false);

            return announcement;
        }

        public static async Task DeleteAnnouncementAsync(
            Guid announcementId,
            IAnnouncementRepository announcementRepository,
            string azureStorageConnectionString)
        {
            await AzureBlobModel.DeleteContentBlobContainerAsync(announcementId, azureStorageConnectionString).ConfigureAwait(false);

            var announcement = await announcementRepository.GetByIdAsync(announcementId);
            
            await announcementRepository.RemoveAsync(announcement);
        }
    }
}
