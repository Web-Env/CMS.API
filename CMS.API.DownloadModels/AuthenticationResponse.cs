namespace CMS.API.DownloadModels
{
    public class AuthenticationResponse
    {
        public string UserId { get; set; }

        public string Token { get; set; }

        public bool Super { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }
}
