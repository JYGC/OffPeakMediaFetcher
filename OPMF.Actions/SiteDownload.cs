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
            Database.DatabaseAdapter.AccessDbAdapter(dbAdapter =>
            {
                dbAdapter.YoutubeChannelDbCollection.InsertOrUpdate(channels);
            });
        }

        public static void ImportChannels(string filePath)
        {
            string opml = File.ReadAllText(filePath);
            SiteAdapter.Youtube.RssChannelImporter youtubeChannelImporter = new SiteAdapter.Youtube.RssChannelImporter(opml);
            IEnumerable<Entities.IChannel> channels = youtubeChannelImporter.ImportChannels();
            Database.DatabaseAdapter.AccessDbAdapter(dbAdapter =>
            {
                dbAdapter.YoutubeChannelDbCollection.InsertOrUpdate(channels);
            });
        }

        public static void FetchMetadata()
        {
            SiteAdapter.ISiteAdapter<Entities.IChannel, Entities.IMetadata> siteAdapter = new SiteAdapter.Youtube.YoutubeAdapter();

            List<Entities.IChannel> channels = null;
            Console.WriteLine("getting channels");
            Database.DatabaseAdapter.AccessDbAdapter(dbConn =>
            {
                channels = new List<Entities.IChannel>(dbConn.YoutubeChannelDbCollection.GetNotBacklisted());
            });
            List<Entities.IMetadata> metadatas = siteAdapter.FetchMetadata(ref channels);
            Console.WriteLine("saving metadata to database");
            Database.DatabaseAdapter.AccessDbAdapter(dbConn =>
            {
                dbConn.YoutubeMetadataDbCollection.InsertNew(metadatas);
                Console.WriteLine("updating channels");
                dbConn.YoutubeChannelDbCollection.UpdateLastCheckedOutAndActivity(channels);
            });
        }

        public static void FetchVideos()
        {
            Downloader.IDownloader<Entities.IMetadata> downloader = new Downloader.YTDownloader.YTDownloader();

            Console.WriteLine("fetching videos");
            List<Entities.IMetadata> metadatas = null;
            Database.DatabaseAdapter.AccessDbAdapter(dbConn =>
            {
                metadatas = new List<Entities.IMetadata>(dbConn.YoutubeMetadataDbCollection.GetToDownload());
            });
            downloader.Download(metadatas);
            Database.DatabaseAdapter.AccessDbAdapter(dbConn =>
            {
                dbConn.YoutubeMetadataDbCollection.UpdateStatus(metadatas);
            });
            FolderSetup.EstablishVideoOutputFolder();
            FileOperations.MoveAllInFolder(Settings.ConfigHelper.ReadonlySettings.GetDownloadFolderPath(),
                                           Settings.ConfigHelper.Config.VideoOutputFolderPath,
                                           new string[]
                                           { 
                                               Settings.ConfigHelper.Config.YoutubeDL.VideoExtension
                                               , Settings.ConfigHelper.Config.YoutubeDL.SubtitleExtension
                                           });
        }
    }
}
