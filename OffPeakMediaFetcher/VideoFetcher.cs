using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OffPeakMediaFetcher
{
    internal class VideoFetcher
    {
        public void Run(string siteId)
        {
            __Run(dbConn => new List<OPMF.Entities.Metadata> { dbConn.YoutubeMetadataDbCollection.GetBySiteId(siteId) });
        }

        public void Run()
        {
            __Run(dbConn => new List<OPMF.Entities.Metadata>(dbConn.YoutubeMetadataDbCollection.GetToDownload()));
        }

        private static void __MoveAllInFolder(string srcFolderPath, string dstFolderPath, string[] fileExtensions)
        {
            IEnumerable<string> srcFilepaths = new string[] { };

            foreach (string fileExtension in fileExtensions)
            {
                srcFilepaths = srcFilepaths.Concat(Directory.GetFiles(srcFolderPath).Where(f => f.EndsWith("." + fileExtension)));
            }

            foreach (string srcFile in srcFilepaths)
            {
                FileInfo srcFileInfo = new FileInfo(srcFile);
                if (new FileInfo(Path.Join(dstFolderPath, srcFileInfo.Name)).Exists == false)
                {
                    srcFileInfo.MoveTo(Path.Join(dstFolderPath, srcFileInfo.Name));
                }
            }
        }

        public void __Run(Func<OPMF.Database.DatabaseAdapter, List<OPMF.Entities.Metadata>> GetVideoMetadata)
        {
            List<OPMF.Entities.Metadata> metadatasForErrorLogging = null;
            try
            {
                OPMF.Downloader.IDownloader downloader = new OPMF.Downloader.YTDownloader.YTDownloader();

                Console.WriteLine("fetching videos");
                OPMF.Database.DatabaseAdapter.AccessDbAdapter(dbConn =>
                {
                    downloader.DownloadQueue = GetVideoMetadata(dbConn);
                    dbConn.YoutubeMetadataDbCollection.UpdateIsBeingProcessed(downloader.DownloadQueue, true);
                });
                metadatasForErrorLogging = downloader.DownloadQueue; // Pass to log if error
                downloader.StartDownloadingQueue();
                OPMF.Database.DatabaseAdapter.AccessDbAdapter(dbConn =>
                {
                    dbConn.YoutubeMetadataDbCollection.UpdateStatus(downloader.DownloadQueue);
                    dbConn.YoutubeMetadataDbCollection.UpdateIsBeingProcessed(downloader.DownloadQueue, false);
                });
                OPMF.Filesystem.FolderSetup.EstablishVideoOutputFolder();
                __MoveAllInFolder(OPMF.Settings.ConfigHelper.ReadonlySettings.GetDownloadFolderPath(),
                                  OPMF.Settings.ConfigHelper.Config.VideoOutputFolderPath,
                                  new string[]
                                  {
                                      OPMF.Settings.ConfigHelper.Config.YoutubeDL.VideoExtension,
                                      OPMF.Settings.ConfigHelper.Config.YoutubeDL.SubtitleExtension
                                  });
            }
            catch (Exception e)
            {
                OPMF.Logging.Logger.GetCurrent().LogEntry(new OPMF.Entities.OPMFError(e)
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
