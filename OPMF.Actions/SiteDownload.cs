using System;
using System.Collections.Generic;
using System.IO;

namespace OPMF.Actions
{
    public static class SiteDownload
    {
        public static void ImportChannels() // Test this
        {
            SiteAdapter.ISiteChannelFinder siteAdapter = null;
            List<Entities.IChannel> channels = null;
            try
            {
                siteAdapter = new SiteAdapter.Youtube.YoutubeChannelFinder();
                channels = siteAdapter.ImportChannels();

                Console.WriteLine("saving channels to database");
                Database.DatabaseAuxillary.RemoveDuplicateIds(channels);
                Database.DatabaseAdapter.AccessDbAdapter(dbAdapter =>
                {
                    dbAdapter.YoutubeChannelDbCollection.InsertOrUpdate(channels);
                });
            }
            catch (Exception e)
            {
                Logging.Logger.GetCurrent().LogEntry(new Entities.OPMFError(e)
                {
                    Variables = new Dictionary<string, object>
                    {
                        { "channels", channels }
                    }
                });
            }
        }

        public static void ImportChannels(string filePath)
        {
            string opml = null;
            IEnumerable<Entities.IChannel> channels = null;
            try
            {
                opml = File.ReadAllText(filePath);
                SiteAdapter.Youtube.RssChannelImporter youtubeChannelImporter = new SiteAdapter.Youtube.RssChannelImporter(opml);
                channels = youtubeChannelImporter.ImportChannels();
                Database.DatabaseAdapter.AccessDbAdapter(dbAdapter =>
                {
                    dbAdapter.YoutubeChannelDbCollection.InsertOrUpdate(channels);
                });
            }
            catch (Exception e)
            {
                Logging.Logger.GetCurrent().LogEntry(new Entities.OPMFError(e)
                {
                    Variables = new Dictionary<string, object>
                    {
                        { "opml", opml },
                        { "channels", channels }
                    }
                });
            }
        }
    }
}
