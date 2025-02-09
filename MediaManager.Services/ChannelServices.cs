using OPMF.Database;
using OPMF.Entities;

namespace MediaManager.Services
{
    public interface IChannelServices
    {
        List<Channel> GetAll();
        void UpdateBlackListStatus(IEnumerable<IChannel> items);
    }
    public class ChannelServices : IChannelServices
    {
        public List<Channel> GetAll()
        {
            List<Channel> channels = [];
            DatabaseAdapter.AccessDbAdapter(dbAdapter =>
            {
                var channelInterfaces = dbAdapter.YoutubeChannelDbCollection.GetAll();
                foreach (IChannel channelInterface in channelInterfaces)
                {
                    channels.Add(new Channel
                    {
                        SiteId = channelInterface.SiteId,
                        BlackListed = channelInterface.BlackListed,
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

        public void UpdateBlackListStatus(IEnumerable<IChannel> items)
        {
            DatabaseAdapter.AccessDbAdapter(dbAdapter =>
            {
                dbAdapter.YoutubeChannelDbCollection.UpdateBlackListStatus(items);
            });
        }
    }
}
