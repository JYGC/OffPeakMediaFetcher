﻿using System;
using System.IO;

namespace OPMF.Settings
{
    public interface IReadonlySettings
    {
        string GetLocalAppFolderPath();
        string GetCredentialPath();
        string GetConfigFilePath();
        string GetDownloadFolderPath();
        string GetDatabaseFolderPath();
        string GetDatabasePath();
        string GetBinFolderPath();
        string GetYoutubeDLPath();
    }

    public class ReadonlySettings : IReadonlySettings
    {
        protected string _appFolderName = "OffPeakMediaFetcher";

        public string GetLocalAppFolderPath()
        {
            return Path.Join(OSCompat.EnvironmentHelper.Environment.GetUserLocalAppFolderPath(), _appFolderName);
        }

        public string GetCredentialPath()
        {
            return Path.Join(GetLocalAppFolderPath(), "Token.json");
        }

        public string GetConfigFilePath()
        {
            return Path.Join(GetLocalAppFolderPath(), "config.json");
        }

        public string GetDownloadFolderPath()
        {
            return Path.Join(GetLocalAppFolderPath(), "Downloads");
        }

        public string GetDatabaseFolderPath()
        {
            return Path.Join(GetLocalAppFolderPath(), "Databases");
        }

        public string GetDatabasePath()
        {
            return Path.Join(GetDatabaseFolderPath(), "OPMF.db");
        }

        public string GetBinFolderPath()
        {
            return Path.Join(GetLocalAppFolderPath(), "Bin");
        }

        public string GetYoutubeDLPath()
        {
            return Path.Join(GetBinFolderPath(), "youtube-dl.exe");
        }
    }

    public class ReadOnlyTestSettings : ReadonlySettings
    {
        public ReadOnlyTestSettings()
        {
            _appFolderName = Path.Join(_appFolderName, "Test");
        }
    }
}
