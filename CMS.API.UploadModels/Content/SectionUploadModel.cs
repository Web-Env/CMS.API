using System;
using System.ComponentModel.DataAnnotations;

namespace CMS.API.UploadModels.Content
{
    public class SectionUploadModel
    {
        public Guid Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Path { get; set; }

        [Required]
        public bool Active { get; set; }
    }
}
