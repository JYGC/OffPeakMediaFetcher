using System;

namespace OPMF.Entities
{
    public interface IChannel : IId
    {
        string SiteId { get; set; }
        string Url { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        bool BlackListed { get; set; }
        bool NotFound { get; set; }
        DateTime? LastCheckedOut { get; set; }
        DateTime? LastActivityDate { get; set; }
    }

    public class Channel : IChannel
    {
        public string Id { get; set; }
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
        public string Url { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool BlackListed { get; set; } = false;
        public bool NotFound { get; set; } = false;
        public DateTime? LastCheckedOut { get; set; }
        public DateTime? LastActivityDate { get; set; }
    }
}
