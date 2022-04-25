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
        private bool __isBeingDownloaded;

        public new MetadataStatus Status
        {
            get
            {
                return __status;
            }
            set
            {
                __status = value;
                NotifyPropertyChanged("DisplayedStatus");
            }
        }

        public new bool IsBeingDownloaded
        {
            get
            {
                return __isBeingDownloaded;
            }
            set
            {
                __isBeingDownloaded = value;
                NotifyPropertyChanged("DisplayedStatus");
            }
        }

        public string DisplayedStatus
        {
            get
            {
                return IsBeingDownloaded ? "IsBeingDownloaded" : __status.ToString();
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
            IsBeingDownloaded = metadata.IsBeingDownloaded;
            PublishedAt = metadata.PublishedAt;
            ChannelSiteId = metadata.ChannelSiteId;
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
