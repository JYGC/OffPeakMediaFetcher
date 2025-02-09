using MediaManager.Services;
using OPMF.Entities;

namespace MediaManagerUI.Modules.ChannelBrowser
{
    public class AllChannelsModule : IChannelBrowserModule
    {
        private readonly IChannelServices _channelServices;

        public bool IsLoading { get; protected set; } = false;
        public List<Channel> Results { get; protected set; } = [];

        public void ScheduleChannelBacklistChange(Channel channel)
        {
            _channelStatusUpdateQueue.Add(channel);
        }

        public async Task GetResultsAsync()
        {
            Results = [];

            IsLoading = true;
            await Task.Run(() =>
            {
                Results = _channelServices.GetAll();
            });

            IsLoading = false;
        }

        public AllChannelsModule(IChannelServices channelServices)
        {
            _channelServices = channelServices;

            _timer = new System.Timers.Timer(_timeBetweenInterval);
            _timer.Elapsed += UpdateChannelStatuses;
            _timer.Enabled = true;
        }

        private readonly List<IChannel> _channelStatusUpdateQueue = [];

        private void UpdateChannelStatuses(object? source, System.Timers.ElapsedEventArgs e)
        {
            if (_channelStatusUpdateQueue.Count == 0)
            {
                return;
            }
            else if (_channelStatusUpdateQueue.Count == 1)
            {
                var channel = _channelStatusUpdateQueue[0];
                _channelStatusUpdateQueue.Remove(channel);
                _channelServices.UpdateBlackListStatus([channel]);
            }
            else
            {
                var channelStatusUpdateQueueSizeMinusOne = _channelStatusUpdateQueue.Count - 1;
                var channels = _channelStatusUpdateQueue.GetRange(0, channelStatusUpdateQueueSizeMinusOne);
                _channelStatusUpdateQueue.RemoveRange(0, channelStatusUpdateQueueSizeMinusOne);
                _channelServices.UpdateBlackListStatus(channels);
            }
        }
        private const int _timeBetweenInterval = 1000;
        private readonly System.Timers.Timer _timer;
    }
}
