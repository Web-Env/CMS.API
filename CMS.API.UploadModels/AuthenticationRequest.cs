using System.ComponentModel.DataAnnotations;

namespace CMS.API.UploadModels
{
    public class AuthenticationRequest
    {
        [Required]
        public string UserAddress { get; set; }

        [Required]
        public string EmailAddress { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
