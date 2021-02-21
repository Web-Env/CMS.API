using System.ComponentModel.DataAnnotations;

namespace CMS.API.UploadModels
{
    public class UploadModelBase : IUploadModel
    {
        [Required]
        public string UserAddress { get; set; }

        [Required]
        public string RequesterUserId { get; set; }
    }
}
