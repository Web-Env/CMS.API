using CMS.API.DownloadModels.User;
using System.Collections.Generic;

namespace CMS.API.DownloadModels.Content
{
    public class SectionDownloadModel : DownloadModelBase
    {
        public string Title { get; set; }

        public string Path { get; set; }

        public IEnumerable<ContentDownloadModel> Contents { get; set; }

        public new UserDownloadModel CreatedBy { get; set; }
    }
}
