using System.IO;

namespace OPMF.Actions
{
    /// <summary>
    /// Exposes methods that alter OS file system for the program.
    /// </summary>
    public static class DirectorySetup
    {
        /// <summary>
        /// Make sure app directory exists. If not create it.
        /// </summary>
        public static void EstablishAppDirectory()
        {
            if (!Directory.Exists(Settings.ReadonlySettings.AppFolderPath))
            {
                Directory.CreateDirectory(Settings.ReadonlySettings.AppFolderPath);
            }
        }
    }
}
