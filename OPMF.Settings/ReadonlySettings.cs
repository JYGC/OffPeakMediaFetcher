using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OPMF.Settings
{
    public static class ReadonlySettings
    {
        private static string __configFileName = "config.json";
        private static string __appFolderName = "OffPeakMediaFetcher";

        public static string ConfigFilePath
        {
            get
            {
                return Path.Join(OSEnvironment.GetExecDirectory(), __configFileName);
            }
        }

        public static string AppFolderPath
        {
            get
            {
                return Path.Join(OSEnvironment.GetUserLocalAppDirectory(), __appFolderName);
            }
        }
    }
}
