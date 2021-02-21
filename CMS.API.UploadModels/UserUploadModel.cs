using System;

namespace CMS.API.UploadModels
{
    public class UserUploadModel : UploadModelBase
    {
        public string Email { get; set; }

        public char[] Password { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public bool IsAdmin { get; set; }

        public DateTime CreatedOn { get; set; }

        public Guid CreatedBy { get; set; }

        public DateTime LastUpdatedOn { get; set; }

        public Guid LastUpdatedBy { get; set; }
    }
}
