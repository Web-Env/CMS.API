using System.ComponentModel.DataAnnotations;

namespace CMS.API.UploadModels.Content
{
    public class SectionUploadModel : UploadModelBase
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string Path { get; set; }

        [Required]
        public bool Active { get; set; }
    }
}
