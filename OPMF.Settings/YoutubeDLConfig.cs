using System;
using System.Collections.Generic;
using System.Text;

namespace OPMF.Settings
{
    public class YoutubeDLConfig
    {
        public bool CheckForBinaryUpdates { get; set; } = true;
        public bool GetSubtitles { get; set; } = true;
        public string OutputExtension { get; set; } = "mp4";
    }
}
