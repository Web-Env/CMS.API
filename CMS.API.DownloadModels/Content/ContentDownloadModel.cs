﻿using CMS.API.DownloadModels.User;

namespace CMS.API.DownloadModels.Content
{
    public class ContentDownloadModel : DownloadModelBase
    {
        public string Title { get; set; }

        public string Path { get; set; }

        public int Views { get; set; }

        public string SectionId { get; set; }

        public SectionDownloadModel Section { get; set; }

        public string Content { get; set; }

        public new UserDownloadModel CreatedBy { get; set; }
    }
}
