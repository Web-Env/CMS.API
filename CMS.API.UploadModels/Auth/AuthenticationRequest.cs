using System.ComponentModel.DataAnnotations;

namespace CMS.API.UploadModels.Auth
{
    public class AuthenticationRequest
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
