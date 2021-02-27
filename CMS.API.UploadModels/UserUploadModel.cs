using System.ComponentModel.DataAnnotations;

namespace CMS.API.UploadModels
{
    public class UserUploadModel : UploadModelBase
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public bool IsAdmin { get; set; }
    }
}
