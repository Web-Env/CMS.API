namespace CMS.API.DownloadModels.User
{
    public class UserDownloadModel : DownloadModelBase
    {
        public string Email { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public bool IsAdmin { get; set; }

        public new string CreatedBy { get; set; }
    }
}
