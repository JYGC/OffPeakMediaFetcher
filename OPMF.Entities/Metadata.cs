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
        string Description { get; set; }
        MetadataStatus Status { get; set; }
        bool IsBeingDownloaded { get; set; }
        DateTime PublishedAt { get; set; }
        string ChannelSiteId { get; set; }
    }

    public class Metadata : IMetadata
    {
        public string Id { get; set; }
        public string Url { get; set; }
        public string SiteId
        {
            get
            {
                return Id;
            }
            set
            {
                Id = value;
            }
        }
        public string Title { get; set; }
        public string Description { get; set; }
        public MetadataStatus Status { get; set; } = MetadataStatus.New;
        public bool IsBeingDownloaded { get; set; } = false;
        public DateTime PublishedAt { get; set; }
        public string ChannelSiteId { get; set; }
    }
}
