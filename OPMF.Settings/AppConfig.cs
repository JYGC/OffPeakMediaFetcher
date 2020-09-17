using System;
using System.IO;

namespace OPMF.Settings
{
    /// <summary>
    /// Stores config steetings.
    /// </summary>
    public class AppConfig
    {
        public int NewChannelPastVideoDayLimit { get; set; } = 1;
        public string GoogleClientSecretPath { get; set; } = "gmail-python-quickstart.json";
        public string VideoOutputFolderPath { get; set; } = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "OffPeakVideos");
        public string WebBrowserPath { get; set; } = @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe";
        public YoutubeDLConfig YoutubeDL { get; set; } = new YoutubeDLConfig();
    }
}
