using System;
using System.IO;

namespace OPMF.Settings
{
    /// <summary>
    /// Stores config steetings.
    /// </summary>
    public class AppConfig
    {
        public int NewChannelPastVideoDayLimit { get; set; } = 28;
        public string GoogleClientSecretPath { get; set; } = "gmail-python-quickstart.json";
        public string VideoOutputFolderPath { get; set; } = Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "OffPeakVideos");
        public YoutubeDLConfig YoutubeDL { get; set; } = new YoutubeDLConfig();
    }
}
