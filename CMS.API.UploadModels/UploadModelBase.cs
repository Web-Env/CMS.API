﻿using System;
using System.ComponentModel.DataAnnotations;

namespace CMS.API.UploadModels
{
    public class UploadModelBase : IUploadModel
    {
        [Required]
        public string UserAddress { get; set; }

        [Required]
        public string RequesterUserId { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }

        [Required]
        public string CreatedBy { get; set; }

        [Required]
        public DateTime LastUpdatedOn { get; set; }

        [Required]
        public string LastUpdatedBy { get; set; }
    }
}
