using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace OPMF.Entities
{
    public interface IStatusChangedMetadata : IMetadata, INotifyPropertyChanged { }

    public class StatusChangedMetadata : Metadata, IStatusChangedMetadata
    {
        private MetadataStatus __status;

        new public MetadataStatus Status
        {
            get
            {
                return __status;
            }
            set
            {
                __status = value;
                NotifyPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public StatusChangedMetadata(IMetadata metadata)
        {
            Id = metadata.Id;
            Url = metadata.Url;
            SiteId = metadata.SiteId;
            Title = metadata.Title;
            Description = metadata.Description;
            Status = metadata.Status;
            PublishedAt = metadata.PublishedAt;
            ChannelSiteId = metadata.ChannelSiteId;
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
