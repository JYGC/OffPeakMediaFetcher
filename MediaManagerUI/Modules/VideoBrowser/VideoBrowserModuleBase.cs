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

        protected Dictionary<string, IMetadata> _metadataIdResultsMap = [];
        private readonly List<IMetadata> _metadataStatusUpdateQueue = [];

        public void ScheduleMetadataStatusUpdate(MetadataStatus newStatus, IMetadata metadata)
        {
            metadata.Status = newStatus;
            _metadataStatusUpdateQueue.Add(metadata);
        }

        private void UpdateMetadataStatuses(object? source, System.Timers.ElapsedEventArgs e)
        {
            if (_metadataStatusUpdateQueue.Count == 0)
            {
                return;
            }
            else if (_metadataStatusUpdateQueue.Count == 1)
            {
                var metadata = _metadataStatusUpdateQueue[0];
                _metadataStatusUpdateQueue.Remove(metadata);
                _metadataServices.UpdateStatus([metadata]);
            }
            else
            {
                var metadataStatusUpdateQueueSizeMinusOne = _metadataStatusUpdateQueue.Count - 1;
                var metadatas = _metadataStatusUpdateQueue.GetRange(0, metadataStatusUpdateQueueSizeMinusOne);
                _metadataStatusUpdateQueue.RemoveRange(0, metadataStatusUpdateQueueSizeMinusOne);
                _metadataServices.UpdateStatus(metadatas);
            }
        }
        private const int _timeBetweenInterval = 1000;
        private readonly System.Timers.Timer _timer;
    }
}
