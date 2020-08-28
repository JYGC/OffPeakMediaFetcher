using System;
using System.Collections.Generic;

namespace OPMF.Actions
{
    public class SiteDownload
    {
        public static void ImportChannels()
        {
            SiteAdapter.ISiteAdapter<Entities.IChannel, Entities.IVideoInfo> siteAdapter = new SiteAdapter.Youtube.YoutubeAdapter();

            List<Entities.IChannel> channels = siteAdapter.ImportChannels();

            Console.WriteLine("saving channels to database");
            Database.DatabaseAuxillary.RemoveDuplicateIds(channels);
            using (Database.IChannelDbAdapter<Entities.IChannel> channelAdapter = new Database.YoutubeChannelDbAdapter())
            {
                channelAdapter.InsertOrUpdate(channels);
            }
            
        }

        public static void FetchVideoInfos()
        {
            SiteAdapter.ISiteAdapter<Entities.IChannel, Entities.IVideoInfo> siteAdapter = new SiteAdapter.Youtube.YoutubeAdapter();

            using (Database.IChannelDbAdapter<Entities.IChannel> channelDbAdapter = new Database.YoutubeChannelDbAdapter())
            {
                List<Entities.IChannel> channels = channelDbAdapter.GetNotBacklisted();
                List<Entities.IVideoInfo> videoInfos = siteAdapter.FetchVideoInfos(ref channels);

                Console.WriteLine("saving video information to database");
                using (Database.IVideoInfoDbAdapter<Entities.IVideoInfo> videoInfoDbAdapter = new Database.YoutubeVideoInfoDbAdapter())
                {
                    videoInfoDbAdapter.InsertOrIgnore(videoInfos);
                }
                Console.WriteLine("updating channels");
                channelDbAdapter.UpdateLastCheckedOutAndActivity(channels);
            }
        }
    }
}
