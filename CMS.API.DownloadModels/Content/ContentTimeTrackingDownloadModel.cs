using System;

namespace CMS.API.DownloadModels.Content
{
    public class ContentTimeTrackingDownloadModel
    {
        public Guid Id { get; set; }

        public Guid ContentId { get; set; }

        public ContentDownloadModel Content { get; set; }

        public Guid UserId { get; set; }

        public int TotalTime { get; set; }

        public DateTime LastSeen { get; set; }
    }
}
