using System;

namespace OPMF.Entities
{
    public class Channel : SiteIdAsId
    {
        public string Url { get; set; }
        public string Name { get; set; }
        public EntityThumbnail Thumbnail { get; set; } = new EntityThumbnail { Url = null, Width = 0, Height = 0 };
        public string Description { get; set; }
        public bool Blacklisted { get; set; } = false;
        public bool IsAddedBySingleVideo { get; set; } = false;
        public bool NotFound { get; set; } = false;
        public DateTime? LastCheckedOut { get; set; }
        public DateTime? LastActivityDate { get; set; }
        public object OriginalResponse { get; set; }
    }
}
