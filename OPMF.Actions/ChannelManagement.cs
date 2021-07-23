using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OPMF.Actions
{
    public static class ChannelManagement
    {
        public static IEnumerable<Entities.IPropertyChangeChannel> GetAllChannels()
        {
            IEnumerable<Entities.IPropertyChangeChannel> channels = new Entities.IPropertyChangeChannel[] { };

            Database.DatabaseAdapter.AccessDbAdapter((dbAdapter) =>
            {
                IEnumerable<Entities.IChannel> rawChannels = dbAdapter.YoutubeChannelDbCollection.GetAll();
                foreach (Entities.IChannel rawChannel in rawChannels)
                {
                    channels = channels.Concat(new Entities.IPropertyChangeChannel[]
                    {
                        new Entities.PropertyChangeChannel(rawChannel)
                    });
                }
            });

            return channels;
        }

        public static void UpdateChannelSettings(IEnumerable<Entities.IChannel> channels)
        {
            Database.DatabaseAdapter.AccessDbAdapter((dbAdapter) =>
            {
                dbAdapter.YoutubeChannelDbCollection.UpdateBlackListStatus(channels);
            });
        }
    }
}
