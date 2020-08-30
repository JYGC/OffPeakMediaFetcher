using System;

namespace OPMF.Entities
{
    public interface IMetadata : IId
    {
        string SiteId { get; set; }
        string Url { get; set; }
        string Title { get; set; }
        string Description { get; set; }
        bool LookedAt { get; set; }
        bool Ignore { get; set; }
        bool Downloaded { get; set; }
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
        public bool LookedAt { get; set; } = false;
        public bool Ignore { get; set; } = true;
        public bool Downloaded { get; set; } = false;
        public DateTime PublishedAt { get; set; }
        public string ChannelSiteId { get; set; }
    }
}
