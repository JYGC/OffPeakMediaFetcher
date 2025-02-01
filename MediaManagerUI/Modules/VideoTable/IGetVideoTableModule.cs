using OPMF.Entities;

namespace MediaManagerUI.Modules.VideoTable
{
    public interface IGetVideoTableModule
    {
        bool IsLoading { get; }
        List<ChannelMetadata> Results { get; }
        MetadataStatus[] UnselectableMetadataStatuses { get; }
        Task GetResultsAsync();
        void ScheduleMetadataStatusUpdate(MetadataStatus newStatus, IMetadata Metadata);
    }
}
