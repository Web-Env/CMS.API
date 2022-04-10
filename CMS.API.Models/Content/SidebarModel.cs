using CMS.API.DownloadModels.Content;
using CMS.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.API.Models.Content
{
    public static class SidebarModel
    {
        public static async Task<List<SidebarButtonDownloadModel>> GetSidebarButtonsAsync(IRepositoryManager repositoryManager)
        {
            var sidebarButtons = new List<SidebarButtonDownloadModel>();
            var contents = await repositoryManager.ContentRepository.GetAllAsync();
            var sections = await repositoryManager.SectionRepository.GetAllAsync();
            var sectionContentIds = new List<Guid>();

            foreach (var section in sections)
            {
                var sectionSidebarButton = new SidebarButtonDownloadModel
                {
                    Title = section.Title,
                    Path = section.Path,
                    CreatedOn = section.CreatedOn
                };
                var sectionContents = contents.Where(c => c.SectionId == section.Id);

                if (sectionContents.Any())
                {
                    foreach (var sectionContent in sectionContents)
                    {
                        sectionSidebarButton.SubButtons.Add(new SidebarButtonDownloadModel
                        {
                            Title = sectionContent.Title,
                            Path = $"{section.Path}/{sectionContent.Path}",
                            CreatedOn = sectionContent.CreatedOn
                        });

                        sectionContentIds.Add(sectionContent.Id);
                    }

                    sidebarButtons.Add(sectionSidebarButton);
                }
            }

            foreach (var content in contents.Where(c => !sectionContentIds.Contains(c.Id)))
            {
                sidebarButtons.Add(new SidebarButtonDownloadModel
                {
                    Title = content.Title,
                    Path = content.Path,
                    CreatedOn = content.CreatedOn
                });
            }

            return sidebarButtons.OrderByDescending(sb => sb.CreatedOn).ToList();
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
