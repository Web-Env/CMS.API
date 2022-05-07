using System;
using System.ComponentModel.DataAnnotations;

namespace CMS.API.UploadModels.Content
{
    public class ContentUploadModel
    {
        public Guid? Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Path { get; set; }

        public Guid? SectionId { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public bool Active { get; set; }
    }
}
