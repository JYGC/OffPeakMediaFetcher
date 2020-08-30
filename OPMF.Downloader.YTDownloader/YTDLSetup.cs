using System;
using System.IO;
using System.Net;

namespace OPMF.Downloader.YTDownloader
{
    static class YTDLSetup
    {
        private static readonly string __youtubeDLUrl = "https://yt-dl.org/latest/youtube-dl.exe";

        public static void EstablishYoutubeDLExec()
        {
            if (!File.Exists(Settings.ReadonlySettings.YoutubeDLPath))
            {
                Console.WriteLine("downloading youtube-dl binary");
                using (WebClient client = new WebClient())
                {
                    client.DownloadFile(__youtubeDLUrl, Settings.ReadonlySettings.YoutubeDLPath);
                }
            }
        }
    }
}
