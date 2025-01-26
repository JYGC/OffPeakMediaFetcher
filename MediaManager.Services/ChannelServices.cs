using OPMF.Database;
using OPMF.Entities;

namespace MediaManager.Services
{
    public interface IChannelServices
    {
        List<Channel> GetAll();
        void UpdateBlackListStatus(IEnumerable<Channel> items);
    }
    public class ChannelServices : IChannelServices
    {
        public List<Channel> GetAll()
        {
            List<Channel> channels = [];
            DatabaseAdapter.AccessDbAdapter(dbAdapter =>
            {
                var channelInterfaces = dbAdapter.YoutubeChannelDbCollection.GetAll();
                foreach (Channel channelInterface in channelInterfaces)
                {
                    channels.Add(new Channel
                    {
                        SiteId = channelInterface.SiteId,
                        Blacklisted = channelInterface.Blacklisted,
                        Description = channelInterface.Description,
                        IsAddedBySingleVideo = channelInterface.IsAddedBySingleVideo,
                        LastActivityDate = channelInterface.LastActivityDate,
                        LastCheckedOut = channelInterface.LastCheckedOut,
                        Name = channelInterface.Name,
                        NotFound = channelInterface.NotFound,
                        Thumbnail = channelInterface.Thumbnail,
                        Url = channelInterface.Url,
                    });
                }
            });
            return channels;
        }

        public void UpdateBlackListStatus(IEnumerable<Channel> items)
        {
            DatabaseAdapter.AccessDbAdapter(dbAdapter =>
            {
                dbAdapter.YoutubeChannelDbCollection.UpdateBlackListStatus(items);
            });
        }
    }
}
