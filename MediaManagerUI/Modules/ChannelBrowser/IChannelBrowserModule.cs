using OPMF.Entities;

namespace MediaManagerUI.Modules.ChannelBrowser
{
    public interface IChannelBrowserModule
    {
        bool IsLoading { get; }
        List<Channel> Results { get; }
        void ScheduleChannelBacklistChange(Channel channel);
        Task GetResultsAsync();
    }
}
