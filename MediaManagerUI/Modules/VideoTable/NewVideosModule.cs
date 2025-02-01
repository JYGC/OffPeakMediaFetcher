using MediaManager.Services;
using OPMF.Entities;

namespace MediaManagerUI.Modules.VideoTable
{
    public class NewVideosModule(IChannelMetadataServices channelMetadataServices) : IAutoGetVideoTableModule
    {
        private const int _pageSize = 10000;
        private int _skip = 0;

        private readonly IChannelMetadataServices _channelMetadataServices = channelMetadataServices;
        public bool IsLoading { get; private set; } = false;
        public List<ChannelMetadata> Results { get; set; } = [];

        public async Task AutoLoadGetResultsAsync()
        {
            await GetResultsAsync();
        }

        public async Task GetResultsAsync()
        {
            Results = [];
            _skip = 0;

            List<ChannelMetadata>? resultsChuck = null;

            IsLoading = true;
            await Task.Run(() =>
            {
                do
                {
                    resultsChuck = _channelMetadataServices.GetNew(_skip, _pageSize);
                    Results.AddRange(resultsChuck);
                    _skip += _pageSize;
                }
                while (resultsChuck != null && resultsChuck.Count() == _pageSize);
            });

            IsLoading = false;
        }
    }
}
