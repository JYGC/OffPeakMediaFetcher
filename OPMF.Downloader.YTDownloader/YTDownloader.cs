using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using NYoutubeDL;

namespace OPMF.Downloader.YTDownloader
{
    public class YTDownloader : IDownloader<Entities.IMetadata>
    {
        private YoutubeDL __youtubeDL;
        private string __downloadError;

        public YTDownloader()
        {
            YTDLSetup.EstablishYoutubeDLExec();
            __youtubeDL = new YoutubeDL();
            __youtubeDL.Options.GeneralOptions.Update = Settings.ConfigHelper.Config.YoutubeDL.CheckForBinaryUpdates;
            __youtubeDL.Options.SubtitleOptions.AllSubs = Settings.ConfigHelper.Config.YoutubeDL.GetSubtitles;
            __youtubeDL.Options.VideoFormatOptions.Format = NYoutubeDL.Helpers.Enums.VideoFormat.best;
            __youtubeDL.YoutubeDlPath = Settings.ReadonlySettings.YoutubeDLPath;

            __youtubeDL.StandardOutputEvent += (sender, output) => Console.WriteLine(output);
            __youtubeDL.StandardErrorEvent += (sender, errorOutput) => __downloadError = errorOutput;
        }

        public void Download(ref List<Entities.IMetadata> items)
        {
            foreach (Entities.IMetadata item in items)
            {
                Console.WriteLine("Downloading: " + item.Title);
                __downloadError = null;
                __youtubeDL.Options.FilesystemOptions.Output = Path.Join(Settings.ReadonlySettings.DownloadFolderPath, item.Title + "." + Settings.ConfigHelper.Config.YoutubeDL.OutputExtension);
                __youtubeDL.Download(item.Url);
                
                if (__downloadError != null)
                {
                    item.Downloaded = true;
                }
                else
                {
                    Console.WriteLine("Error: " + __downloadError);
                }
            }
        }
    }
}
