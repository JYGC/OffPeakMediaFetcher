using MediaManager.Services;
using OPMF.Entities;

namespace MediaManagerUI.Modules.VideoBrowser
{
    public class VideoBrowserModuleBase
    {
        private readonly IMetadataServices _metadataServices;

        protected const int _pageSize = 10000;
        protected int _skip;

        public VideoBrowserModuleBase(IMetadataServices metadataServices)
        {
            _metadataServices = metadataServices;

            _timer = new System.Timers.Timer(_timeBetweenInterval);
            _timer.Elapsed += UpdateMetadataStatuses;
            _timer.Enabled = true;
        }

        public bool IsLoading { get; protected set; } = false;
        public List<ChannelMetadata> Results { get; protected set; } = [];

        protected Dictionary<string, Metadata> _metadataIdResultsMap = [];
        private readonly List<Metadata> _metadataStatusUpdateQueue = [];

        public void ScheduleMetadataStatusUpdate(MetadataStatus newStatus, Metadata metadata)
        {
            metadata.Status = newStatus;
            _metadataStatusUpdateQueue.Add(metadata);
        }

        public void RemoveIsBeingDownloadedStatus(ChannelMetadata channelMetadata)
        {
            channelMetadata.Metadata.IsBeingDownloaded = false;
            _metadataServices.UpdateIsBeingProcessed([channelMetadata.Metadata]);
        }

        public void DownloadVideoNow(ChannelMetadata channelMetadata)
        {
            channelMetadata.Metadata.IsBeingDownloaded = true;
            _metadataServices.DownloadNow(channelMetadata.Metadata);
        }

        private void UpdateMetadataStatuses(object? source, System.Timers.ElapsedEventArgs e)
        {
            if (_metadataStatusUpdateQueue.Count == 0)
            {
                return;
            }
            else
            {
                var metadataStatusUpdateQueueSize = _metadataStatusUpdateQueue.Count;
                var metadatas = _metadataStatusUpdateQueue.GetRange(0, metadataStatusUpdateQueueSize);
                _metadataStatusUpdateQueue.RemoveRange(0, metadataStatusUpdateQueueSize);
                _metadataServices.UpdateStatus(metadatas);
            }
        }
        private const int _timeBetweenInterval = 1000;
        private readonly System.Timers.Timer _timer;
    }
}
