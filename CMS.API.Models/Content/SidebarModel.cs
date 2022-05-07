using CMS.API.DownloadModels.Content;
using CMS.API.Models.User;
using CMS.Domain.Entities;
using CMS.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.API.Models.Content
{
    public static class SidebarModel
    {
        public static async Task<List<SidebarButtonDownloadModel>> GetSidebarButtonsAsync(
            IRepositoryManager repositoryManager,
            Guid requesterId)
        {
            var contents = await repositoryManager.ContentRepository.GetAllAsync();
            var sections = await repositoryManager.SectionRepository.GetAllAsync();
            var userIsAdmin = await UserModel.CheckUserIsAdminByIdAsync(requesterId, repositoryManager.UserRepository)
                .ConfigureAwait(false);

            return MapSidebarButtonsToSidebarButtonDownloadModels(
                sections,
                contents,
                userIsAdmin);
        }

        public static List<SidebarButtonDownloadModel> MapSidebarButtonsToSidebarButtonDownloadModels(
            IEnumerable<Section> sections,
            IEnumerable<Domain.Entities.Content> contents,
            bool userIsAdmin)
        {
            var sectionContentIds = new List<Guid>();

            var sidebarButtons = MapContentToSections(
                sections,
                contents,
                sectionContentIds);

            foreach (var content in contents.Where(c => !sectionContentIds.Contains(c.Id)))
            {
                sidebarButtons.Add(MapContentToSidebarButtonDownloadModel(content));
            }

            sidebarButtons = sidebarButtons.OrderByDescending(sb => sb.CreatedOn).ToList();

            if (userIsAdmin)
            {
                sidebarButtons.Add(GetAdminSidebarButtons());
            }

            return sidebarButtons;
        }

        public static List<SidebarButtonDownloadModel> MapContentToSections(
            IEnumerable<Section> sections,
            IEnumerable<Domain.Entities.Content> contents,
            List<Guid> sectionContentIds)
        {
            var sidebarButtons = new List<SidebarButtonDownloadModel>();

            foreach (var section in sections)
            {
                var sectionSidebarButton = MapSectionToSidebarButtonDownloadModel(section);
                var sectionContents = contents.Where(c => c.SectionId == section.Id);

                if (sectionContents.Any())
                {
                    foreach (var sectionContent in sectionContents)
                    {
                        sectionSidebarButton.SubButtons.Add(MapContentToSidebarButtonDownloadModel(sectionContent, section.Path));
                        sectionContentIds.Add(sectionContent.Id);
                    }

                    sidebarButtons.Add(sectionSidebarButton);
                }
            }

            return sidebarButtons;
        }

        public static SidebarButtonDownloadModel MapSectionToSidebarButtonDownloadModel(Section section)
        {
            return new SidebarButtonDownloadModel
            {
                Title = section.Title,
                Path = section.Path,
                CreatedOn = section.CreatedOn
            };
        }

        public static SidebarButtonDownloadModel MapContentToSidebarButtonDownloadModel(Domain.Entities.Content content)
        {
            return new SidebarButtonDownloadModel
            {
                Title = content.Title,
                Path = content.Path,
                CreatedOn = content.CreatedOn
            };
        }

        public static SidebarButtonDownloadModel MapContentToSidebarButtonDownloadModel(
            Domain.Entities.Content content,
            string sectionPath)
        {
            return new SidebarButtonDownloadModel
            {
                Title = content.Title,
                Path = $"{sectionPath}/{content.Path}",
                CreatedOn = content.CreatedOn
            };
        }

        public static SidebarButtonDownloadModel GetAdminSidebarButtons()
        {
            var adminSidebarButtons = new SidebarButtonDownloadModel
            {
                Title = "Admin",
                Path = "admin",
                CreatedOn = DateTime.Now,
                SubButtons = new List<SidebarButtonDownloadModel>
                {
                    new SidebarButtonDownloadModel
                    {
                        Title = "Users",
                        Path = "admin/users",
                        CreatedOn = DateTime.Now
                    },
                    new SidebarButtonDownloadModel
                    {
                        Title = "Announcements",
                        Path = "admin/announcements",
                        CreatedOn = DateTime.Now
                    },
                    new SidebarButtonDownloadModel
                    {
                        Title = "Sections",
                        Path = "admin/sections",
                        CreatedOn = DateTime.Now
                    },
                    new SidebarButtonDownloadModel
                    {
                        Title = "Content",
                        Path = "admin/content",
                        CreatedOn = DateTime.Now
                    }
                }
            };

            return adminSidebarButtons;
        }
    }
}
