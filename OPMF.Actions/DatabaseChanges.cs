using System;
using System.Collections.Generic;
using System.Text;

namespace OPMF.Actions
{
    public class DatabaseChanges
    {
        public static void Migrate()
        {
            using (var channelDbAdapter = new Database.YoutubeChannelDbAdapter())
            using (var videoInfoDbAdapter = new Database.YoutubeMetadataDbAdapter())
            {
                channelDbAdapter.MigrateData();
                videoInfoDbAdapter.MigrateData();
            }
        }
    }
}
