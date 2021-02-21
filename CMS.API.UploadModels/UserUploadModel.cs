using System;
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

        [Required]
        public DateTime CreatedOn { get; set; }

        [Required]
        public Guid CreatedBy { get; set; }

        public DateTime LastUpdatedOn { get; set; }

        public Guid LastUpdatedBy { get; set; }
    }
}
