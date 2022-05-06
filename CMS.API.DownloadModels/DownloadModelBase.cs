using System;

namespace CMS.API.DownloadModels
{
    public class DownloadModelBase : IDownloadModel
    {
        public string Id { get; set; }

        public DateTime CreatedOn { get; set; }

        public virtual Guid CreatedBy { get; set; }

        public DateTime LastUpdatedOn { get; set; }

        public Guid LastUpdatedBy { get; set; }
    }
}
