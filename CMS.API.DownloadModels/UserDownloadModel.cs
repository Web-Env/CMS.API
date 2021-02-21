﻿using System;

namespace CMS.API.DownloadModels
{
    public class UserDownloadModel : DownloadModelBase
    {
        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public bool IsAdmin { get; set; } = false;

        public DateTime CreatedOn { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime LastUpdatedOn { get; set; }

        public Guid LastUpdatedBy { get; set; }
    }
}