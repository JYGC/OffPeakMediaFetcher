using System;

namespace OPMF.Entities
{
    public interface IChannel
    {
        string SiteId { get; set; }
        string Name { get; set; }
        string Description { get; set; }
        bool BlackListed { get; set; }
        DateTime LastCheckedOut { get; set; }
    }

    public class Channel : IChannel, IId
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
        public string Name { get; set; }
        public string Description { get; set; }
        public bool BlackListed { get; set; } = false;
        public DateTime LastCheckedOut { get; set; }
    }
}
