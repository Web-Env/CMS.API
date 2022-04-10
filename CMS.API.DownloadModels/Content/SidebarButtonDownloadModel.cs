using System;
using System.Collections.Generic;

namespace CMS.API.DownloadModels.Content
{
    public class SidebarButtonDownloadModel
    {
        public string Title { get; set; }

        public string Path { get; set; }

        public DateTime CreatedOn { get; set; }

        public List<SidebarButtonDownloadModel> SubButtons { get; set; } = new List<SidebarButtonDownloadModel>();
    }
}
