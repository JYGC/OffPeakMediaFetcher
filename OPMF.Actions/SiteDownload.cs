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

        public static void FetchMetadata()
        {
            List<Entities.IChannel> channels = null;
            List<Entities.IMetadata> metadatas = null;
            try
            {
                SiteAdapter.IMetadataFetcher<Entities.IChannel, Entities.IMetadata> siteAdapter = new SiteAdapter.Youtube.YoutubeMetadataFetcher();

                Console.WriteLine("getting channels");
                Database.DatabaseAdapter.AccessDbAdapter(dbConn =>
                {
                    channels = new List<Entities.IChannel>(dbConn.YoutubeChannelDbCollection.GetNotBacklisted());
                });
                metadatas = siteAdapter.FetchMetadata(ref channels);
                Console.WriteLine("saving metadata to database");
                Database.DatabaseAdapter.AccessDbAdapter(dbConn =>
                {
                    dbConn.YoutubeMetadataDbCollection.InsertNew(metadatas);
                    Console.WriteLine("updating channels");
                    dbConn.YoutubeChannelDbCollection.UpdateLastCheckedOutAndActivity(channels);
                });
            }
            catch (Exception e)
            {
                Logging.Logger.GetCurrent().LogEntry(new Entities.OPMFError(e)
                {
                    Variables = new Dictionary<string, object>
                    {
                        { "channels", channels },
                        { "metadatas", metadatas }
                    }
                });
                throw e;
            }
        }

        public static void FetchVideos(string siteId)
        {
            __FetchVideos(dbConn => new List<Entities.IMetadata> { dbConn.YoutubeMetadataDbCollection.FindById(siteId) });
        }

        public static void FetchVideos()
        {
            __FetchVideos(dbConn => new List<Entities.IMetadata>(dbConn.YoutubeMetadataDbCollection.GetToDownload()));
        }

        public static void __FetchVideos(Func<Database.DatabaseAdapter, List<Entities.IMetadata>> GetVideoMetadata)
        {
            List<Entities.IMetadata> metadatasForErrorLogging = null;
            try
            {
                Downloader.IDownloader<Entities.IMetadata> downloader = new Downloader.YTDownloader.YTDownloader();

                Console.WriteLine("fetching videos");
                Database.DatabaseAdapter.AccessDbAdapter(dbConn =>
                {
                    downloader.DownloadQueue = GetVideoMetadata(dbConn);
                    dbConn.YoutubeMetadataDbCollection.UpdateIsBeingProcessed(downloader.DownloadQueue, true);
                });
                metadatasForErrorLogging = downloader.DownloadQueue; // Pass to log if error
                downloader.StartDownloadingQueue();
                Database.DatabaseAdapter.AccessDbAdapter(dbConn =>
                {
                    dbConn.YoutubeMetadataDbCollection.UpdateStatus(downloader.DownloadQueue);
                    dbConn.YoutubeMetadataDbCollection.UpdateIsBeingProcessed(downloader.DownloadQueue, false);
                });
                FolderSetup.EstablishVideoOutputFolder();
                FileOperations.MoveAllInFolder(Settings.ConfigHelper.ReadonlySettings.GetDownloadFolderPath(),
                                               Settings.ConfigHelper.Config.VideoOutputFolderPath,
                                               new string[]
                                               {
                                                   Settings.ConfigHelper.Config.YoutubeDL.VideoExtension,
                                                   Settings.ConfigHelper.Config.YoutubeDL.SubtitleExtension
                                               });
            }
            catch (Exception e)
            {
                Logging.Logger.GetCurrent().LogEntry(new Entities.OPMFError(e)
                {
                    Variables = new Dictionary<string, object>
                    {
                        { "metadatas", metadatasForErrorLogging }
                    }
                });
                throw e;
            }
        }
    }
}
