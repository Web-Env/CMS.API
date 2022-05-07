using System;
using System.ComponentModel.DataAnnotations;

namespace CMS.API.UploadModels.User
{
    public class UserUploadModel : UploadModelBase
    {
        public Guid Id { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public DateTime? ExpiresOn { get; set; }

        public bool IsAdmin { get; set; }

        public string AdminPassword { get; set; }
    }
}
