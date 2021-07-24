using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace OPMF.Entities
{
    public interface IPropertyChangedMetadata : IMetadata, INotifyPropertyChanged { }

    /// <summary>
    /// Need to inherit from INotifyPropertyChanged so DataGrid rows can change when states are updated
    /// </summary>
    public class PropertyChangedMetadata : Metadata, IPropertyChangedMetadata
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

        public PropertyChangedMetadata(IMetadata metadata)
        {
            SiteId = metadata.SiteId;
            Url = metadata.Url;
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
