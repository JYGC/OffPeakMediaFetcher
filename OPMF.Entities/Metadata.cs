using System;

namespace OPMF.Entities
{
    public enum MetadataStatus
    {
        New,
        ToDownload,
        Wait,
        Ignore,
        Downloaded
    }

    public interface IMetadata : IStringId
    {
        string SiteId { get; set; }
        string Url { get; set; }
        string Title { get; set; }
        EntityThumbnail Thumbnail { get; set; }
        string Description { get; set; }
        MetadataStatus Status { get; set; }
        bool IsBeingDownloaded { get; set; }
        DateTime PublishedAt { get; set; }
        string ChannelSiteId { get; set; }
    }

    public class Metadata : SiteIdAsId, IMetadata
    {
        public string Url { get; set; }
        public string Title { get; set; }
        public EntityThumbnail Thumbnail { get; set; } = new EntityThumbnail { Url = null, Width = 0, Height = 0 };
        public string Description { get; set; }
        public MetadataStatus Status { get; set; } = MetadataStatus.New;
        public bool IsBeingDownloaded { get; set; } = false;
        public DateTime PublishedAt { get; set; }
        public string ChannelSiteId { get; set; }
    }
}
