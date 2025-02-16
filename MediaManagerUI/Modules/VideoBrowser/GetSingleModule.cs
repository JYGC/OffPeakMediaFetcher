using MediaManager.Services2;
using OPMF.Entities;

namespace MediaManagerUI.Modules.VideoBrowser
{
    public class GetSingleModule(
        IMetadataServices metadataServices,
        IChannelMetadataServices channelMetadataServices) : VideoBrowserModuleBase(metadataServices), IVideoBrowserModule, INetSearchableVideoBrowserModule
    {
        private readonly IChannelMetadataServices _channelMetadataServices = channelMetadataServices;

        public string? SearchVideoUrl { get; set; }
        public MetadataStatus[] UnselectableMetadataStatuses => [MetadataStatus.Downloaded];

        public async Task GetResultsAsync()
        {
            if (string.IsNullOrWhiteSpace(SearchVideoUrl))
            {
                return;
            }

            Results = [];
            _skip = 0;

            List<ChannelMetadata>? resultsChuck = null;

            IsLoading = true;
            try
            {
                await Task.Run(() =>
                {
                    resultsChuck = _channelMetadataServices.GetVideoByUrl(SearchVideoUrl, _skip, _pageSize);
                    Results.AddRange(resultsChuck);
                    _metadataIdResultsMap = Results.ToDictionary(cm => cm.Metadata.Id, cm => cm.Metadata);
                });
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
