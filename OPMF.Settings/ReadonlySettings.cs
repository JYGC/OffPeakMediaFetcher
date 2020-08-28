using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OPMF.Settings
{
    public static class ReadonlySettings
    {
        private static string __appFolderName = "OffPeakMediaFetcher";

        public static string ConfigFilePath { get; } = "config.json";

        public static string AppFolderPath
        {
            get
            {
                return Path.Join(OSEnvironment.GetUserLocalAppDirectory(), __appFolderName);
            }
        }
    }
}
