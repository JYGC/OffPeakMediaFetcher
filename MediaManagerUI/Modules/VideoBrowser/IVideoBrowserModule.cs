using OPMF.Entities;

namespace MediaManagerUI.Modules.VideoBrowser
{
    public interface IVideoBrowserModule
    {
        bool IsLoading { get; }
        List<ChannelMetadata> Results { get; }
        MetadataStatus[] UnselectableMetadataStatuses { get; }
        Task GetResultsAsync();
        void ScheduleMetadataStatusUpdate(MetadataStatus newStatus, Metadata Metadata);
        void RemoveIsBeingDownloadedStatus(ChannelMetadata channelMetadata);
        void DownloadVideoNow(ChannelMetadata channelMetadata);
    }
}
