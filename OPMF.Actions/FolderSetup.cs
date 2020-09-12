using System.IO;

namespace OPMF.Actions
{
    /// <summary>
    /// Exposes methods that alter OS file system for the program.
    /// </summary>
    public static class FolderSetup
    {
        private static void __EstablishFolder(string dirPath)
        {
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
        }

        /// <summary>
        /// Make sure app directory exists. If not create it.
        /// </summary>
        private static void __EstablishAppFolder()
        {
            __EstablishFolder(Settings.ReadonlySettings.LocalAppFolderPath);
        }

        private static void __EstablishDownloadFolder()
        {
            __EstablishFolder(Settings.ReadonlySettings.DownloadFolderPath);
        }

        private static void __EstablishDatabaseFolder()
        {
            __EstablishFolder(Settings.ReadonlySettings.DatabaseFolderPath);
        }

        private static void __EstablishBinFolder()
        {
            __EstablishFolder(Settings.ReadonlySettings.BinFolderPath);
        }

        public static void EstablishFolders()
        {
            __EstablishAppFolder();
            __EstablishDownloadFolder();
            __EstablishDatabaseFolder();
            __EstablishBinFolder();
        }

        public static void EstablishVideoOutputFolder()
        {
            __EstablishFolder(Settings.ConfigHelper.Config.VideoOutputFolderPath);
        }
    }
}
