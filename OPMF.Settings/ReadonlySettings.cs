using System;
using System.IO;

namespace OPMF.Settings
{
    public static class ReadonlySettings
    {
        private static string __appFolderName = "OffPeakMediaFetcher";

        public static string GetLocalAppFolderPath()
        {
            return Path.Join(OSCompat.EnvironmentHelper.Environment.GetUserLocalAppFolderPath(), __appFolderName);
        }

        public static string GetCredentialPath()
        {
            return Path.Join(GetLocalAppFolderPath(), "Token.json");
        }

        public static string GetConfigFilePath()
        {
            return Path.Join(GetLocalAppFolderPath(), "config.json");
        }

        public static string GetDownloadFolderPath()
        {
            return Path.Join(GetLocalAppFolderPath(), "Downloads");
        }

        public static string GetDatabaseFolderPath()
        {
            return Path.Join(GetLocalAppFolderPath(), "Databases");
        }

        public static string GetDatabasePath()
        {
            return Path.Join(GetDatabaseFolderPath(), "OPMF.db");
        }

        public static string GetBinFolderPath()
        {
            return Path.Join(GetLocalAppFolderPath(), "Bin");
        }

        public static string GetYoutubeDLPath()
        {
            return Path.Join(GetBinFolderPath(), "youtube-dl.exe");
        }
    }
}
