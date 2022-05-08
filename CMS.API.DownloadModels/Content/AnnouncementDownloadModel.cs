using CMS.API.DownloadModels.User;

namespace CMS.API.DownloadModels.Content
{
    public class AnnouncementDownloadModel : DownloadModelBase
    {
        public string Title { get; set; }

        public string Path { get; set; }

        public int Views { get; set; }

        public string Content { get; set; }

        public new UserDownloadModel CreatedBy { get; set; }
    }
}
