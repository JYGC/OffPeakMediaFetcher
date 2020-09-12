using System;
using System.Collections.Generic;
using System.IO;

namespace OPMF.Actions
{
    public static class SiteDownload
    {
        public static void ImportChannels()
        {
            SiteAdapter.ISiteAdapter<Entities.IChannel, Entities.IMetadata> siteAdapter = new SiteAdapter.Youtube.YoutubeAdapter();

            List<Entities.IChannel> channels = siteAdapter.ImportChannels();

            Console.WriteLine("saving channels to database");
            Database.DatabaseAuxillary.RemoveDuplicateIds(channels);
            using (Database.IChannelDbAdapter<Entities.IChannel> channelAdapter = new Database.YoutubeChannelDbAdapter())
            {
                channelAdapter.InsertOrUpdate(channels);
            }
        }

        public static void ImportChannels(string filePath)
        {
            string opml = File.ReadAllText(filePath);
            SiteAdapter.Youtube.RssChannelImporter youtubeChannelImporter = new SiteAdapter.Youtube.RssChannelImporter(opml);
            IEnumerable<Entities.IChannel> channels = youtubeChannelImporter.ImportChannels();
            using (Database.IChannelDbAdapter<Entities.IChannel> channelAdapter = new Database.YoutubeChannelDbAdapter())
            {
                channelAdapter.InsertOrUpdate(channels);
            }
        }

        public static void FetchMetadata()
        {
            SiteAdapter.ISiteAdapter<Entities.IChannel, Entities.IMetadata> siteAdapter = new SiteAdapter.Youtube.YoutubeAdapter();

            using (Database.IChannelDbAdapter<Entities.IChannel> channelDbAdapter = new Database.YoutubeChannelDbAdapter())
            {
                List<Entities.IChannel> channels = new List<Entities.IChannel>(channelDbAdapter.GetNotBacklisted());
                List<Entities.IMetadata> metadatas = siteAdapter.FetchMetadata(ref channels);

                Console.WriteLine("saving metadata to database");
                using (Database.IMetadataDbAdapter<Entities.IMetadata> metadataDbAdapter = new Database.YoutubeMetadataDbAdapter())
                {
                    metadataDbAdapter.InsertNew(metadatas);
                }
                Console.WriteLine("updating channels");
                channelDbAdapter.UpdateLastCheckedOutAndActivity(channels);
            }
        }

        public static void FetchVideos()
        {
            Downloader.IDownloader<Entities.IMetadata> downloader = new Downloader.YTDownloader.YTDownloader();

            Console.WriteLine("fetching videos");
            using (Database.IMetadataDbAdapter<Entities.IMetadata> metadataDbAdapter = new Database.YoutubeMetadataDbAdapter())
            {
                List<Entities.IMetadata> metadatas = new List<Entities.IMetadata>(metadataDbAdapter.GetReallyForDownload());
                downloader.Download(ref metadatas);
                metadataDbAdapter.UpdateStatus(metadatas);
            }
            Actions.FolderSetup.EstablishVideoOutputFolder();
            Actions.FileOperations.MoveAllInFolder(Settings.ReadonlySettings.DownloadFolderPath,
                                                   Settings.ConfigHelper.Config.VideoOutputFolderPath,
                                                   Settings.ConfigHelper.Config.YoutubeDL.OutputExtension);
        }
    }
}
