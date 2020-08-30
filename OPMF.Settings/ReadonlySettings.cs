using System;
using System.IO;
using System.Threading.Tasks.Dataflow;

namespace OPMF.Settings
{
    public static class ReadonlySettings
    {
        private static string __appFolderName = "OffPeakMediaFetcher";

        public static string AppFolderPath
        {
            get
            {
                return Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), __appFolderName);
            }
        }

        public static string TempFolderPath
        {
            get
            {
                return Path.Join(Path.GetTempPath(), __appFolderName);
            }
        }

        public static string CredentialPath
        {
            get
            {
                return Path.Join(AppFolderPath, "Token.json");
            }
        }

        public static string ConfigFilePath {
            get
            {
                return Path.Join(AppFolderPath, "config.json");
            }
        }

        public static string DownloadFolderPath
        {
            get
            {
                return Path.Join(TempFolderPath, "Downloads");
            }
        }

        public static string DatabaseFolderPath
        {
            get
            {
                return Path.Join(AppFolderPath, "Databases");
            }
        }

        public static string BinFolderPath
        {
            get
            {
                return Path.Join(AppFolderPath, "Bin");
            }
        }

        public static string YoutubeDLPath
        {
            get
            {
                return Path.Join(BinFolderPath, "youtube-dl.exe");
            }
        }
    }
}
