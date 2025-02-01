using MediaManager.Services;
using OPMF.Entities;

namespace MediaManagerUI.Modules.VideoTable
{
    public class FindVideosModule(IChannelMetadataServices channelMetadataServices) : IGetVideoTableModule, IDatabaseSearchableGetVideoTableModule
    {
        private const int _pageSize = 10000;
        private int _skip = 0;

        private readonly IChannelMetadataServices _channelMetadataServices = channelMetadataServices;

        public bool IsLoading { get; private set; } = false;
        public List<ChannelMetadata> Results { get; private set; } = [];
        public string? SearchVideoTitle { get; set; }
        public string? SearchChannelName { get; set; }

        public async Task GetResultsAsync()
        {
            if (string.IsNullOrWhiteSpace(SearchVideoTitle) && string.IsNullOrWhiteSpace(SearchChannelName))
            {
                return;
            }

            Results = [];
            _skip = 0;

            List<ChannelMetadata>? resultsChuck = null;

            IsLoading = true;
            await Task.Run(() =>
            {
                do
                {
                    resultsChuck = string.IsNullOrWhiteSpace(SearchChannelName)
                        ? _channelMetadataServices.GetByTitleContainingWord(SearchVideoTitle, _skip, _pageSize)
                        : _channelMetadataServices.GetByChannelAndTitleContainingWord(SearchChannelName, SearchVideoTitle,
                            _skip, _pageSize);
                    Results.AddRange(resultsChuck);
                    _skip += _pageSize;
                }
                while (resultsChuck != null && resultsChuck.Count() == _pageSize);
            });
            IsLoading = false;
        }
    }
}
