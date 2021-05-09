namespace OPMF.Settings
{
    public class YoutubeDLConfig
    {
        public bool CheckForBinaryUpdates { get; set; } = true;
        public bool GetSubtitles { get; set; } = false;
        public string VideoExtension { get; set; } = "mp4";
        public string SubtitleExtension { get; set; } = "vtt";
    }
}
