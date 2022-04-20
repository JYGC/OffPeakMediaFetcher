using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using NYoutubeDL;

namespace OPMF.Downloader.YTDownloader
{
    public class YTDownloader : IDownloader<Entities.IMetadata>
    {
        private bool __isDownloading = false;

        public List<Entities.IMetadata> DownloadQueue { get; set; }
        public List<DownloadInstance> DownloadInstances { get; set; }

        public YTDownloader()
        {
            // Setup YoutubeDL
            YTDLSetup.EstablishYoutubeDLExec();
            YoutubeDL youtubeDL = new YoutubeDL();
            youtubeDL.Options.GeneralOptions.Update = Settings.ConfigHelper.Config.YoutubeDL.CheckForBinaryUpdates;

            youtubeDL.StandardOutputEvent += (sender, output) => Console.WriteLine(output);
            youtubeDL.StandardErrorEvent += (sender, errorOutput) => Console.WriteLine(errorOutput);
        }

        public class DownloadInstance
        {
            private const int TITLE_DISPLAY_LENGTH = 40;

            private YoutubeDL __youtubeDL;
            private string __downloadError;
            private string __title;

            public bool NotDownloading { get; set; } = true;
            public int ScreenPosition { get; set; }

            public DownloadInstance()
            {
                __youtubeDL = new YoutubeDL();
                __youtubeDL.Options.SubtitleOptions.AllSubs = Settings.ConfigHelper.Config.YoutubeDL.GetSubtitles;
                __youtubeDL.Options.VideoFormatOptions.Format = NYoutubeDL.Helpers.Enums.VideoFormat.best;
                __youtubeDL.YoutubeDlPath = Settings.ConfigHelper.ReadonlySettings.GetYoutubeDLPath();

                __youtubeDL.StandardOutputEvent += (sender, output) =>
                {
                    WriteOnLine(ScreenPosition, output.StartsWith("[download]") ? string.Format("{0} Video: {1}...", output, __TruncateStrIfTooLong(__title)) : output);
                };
                __youtubeDL.StandardErrorEvent += (sender, errorOutput) => __downloadError = errorOutput;
            }

            private string __TruncateStrIfTooLong(string str)
            {
                return str.Length <= TITLE_DISPLAY_LENGTH ? str : str.Substring(0, TITLE_DISPLAY_LENGTH);
            }

            public void Download(Entities.IMetadata metadata)
            {
                NotDownloading = false;
                __downloadError = null;
                __youtubeDL.Options.FilesystemOptions.Output = Path.Join(Settings.ConfigHelper.ReadonlySettings.GetDownloadFolderPath(),
                                                                         __ItemNameSanitizer(metadata.Title) + "." + Settings.ConfigHelper.Config.YoutubeDL.VideoExtension);
                Thread thread = new Thread(() =>
                {
                    __title = metadata.Title;
                    __youtubeDL.Download(metadata.Url);
                    if (string.IsNullOrEmpty(__downloadError))
                    {
                        metadata.Status = Entities.MetadataStatus.Downloaded;
                    }
                    else
                    {
                        Logging.Logger.GetCurrent().LogEntry(new Entities.OPMFLog
                        {
                            Message = __downloadError,
                            Type = Entities.OPMFLogType.Warning,
                            Variables = new Dictionary<string, object>
                            {
                                { "metadata.Title", __title },
                                { "metadata.Url", metadata.Url }
                            }
                        });
                        WriteOnLine(ScreenPosition, "An error has occured. See error log...");
                    }
                    NotDownloading = true;
                });
                thread.Start();
            }

            private string __ItemNameSanitizer(string itemName)
            {
                string sanitizeredItemName = itemName.Replace("/", "forward slash");
                sanitizeredItemName = sanitizeredItemName.Replace("\"", "''");
                sanitizeredItemName = sanitizeredItemName.Replace("%", "percent");

                return sanitizeredItemName;
            }
        }

        public void AddToDownloadQueue(Entities.IMetadata metadata)
        {
            DownloadQueue.Add(metadata);
            if (DownloadQueue.Count < Settings.ConfigHelper.Config.YoutubeDL.MaxNoOfParallelDownloads)
            {
                int newScreenPosition = DownloadInstances.Count;
                DownloadInstances.Add(new DownloadInstance() { ScreenPosition = newScreenPosition });
            }
            if (!__isDownloading)
            {
                StartDownloadingQueue();
            }
        }

        public void StartDownloadingQueue()
        {
            __isDownloading = true;
            // Create download instance
            DownloadInstances = new List<DownloadInstance>();
            int noOfInstances = (
                DownloadQueue.Count < Settings.ConfigHelper.Config.YoutubeDL.MaxNoOfParallelDownloads
            ) ? DownloadQueue.Count : Settings.ConfigHelper.Config.YoutubeDL.MaxNoOfParallelDownloads;
            for (int i = 0; i < noOfInstances; i++)
            {
                DownloadInstances.Add(new DownloadInstance() { ScreenPosition = i });
            }

            int currentMetadataIndex = 0;
            while (currentMetadataIndex < DownloadQueue.Count || DownloadInstances.Count > 0)
            {
                // Needs to decrement as we are removing elements from instances
                for (int i = DownloadInstances.Count - 1; i >= 0; i--)
                {
                    // Leave instances alone that are still downloading
                    if (DownloadInstances[i].NotDownloading)
                    {
                        if (currentMetadataIndex < DownloadQueue.Count)
                        {
                            DownloadInstances[i].Download(DownloadQueue[currentMetadataIndex]);
                            currentMetadataIndex++;
                        }
                        else
                        {
                            // When there is no more videos to download, remove instances that are not downloading
                            WriteOnLine(DownloadInstances[i].ScreenPosition, "download instance not needed. removed...");
                            DownloadInstances.Remove(DownloadInstances[i]);
                        }
                    }
                }
                Thread.Sleep(5000);
            }
            __isDownloading = false;
        }

        public static void WriteOnLine(int position, string message)
        {
            Console.SetCursorPosition(0, position);
            Console.WriteLine(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, position);
            Console.WriteLine(message);
        }
    }
}
