using System;
using System.IO;

namespace OPMF.Tests
{
    public class AppFolderFixture : IDisposable
    {
        public AppFolderFixture()
        {
            Settings.ConfigHelper.ReadonlySettings = new Settings.ReadOnlyTestSettings();
            Actions.FolderSetup.EstablishFolders();
            Settings.ConfigHelper.EstablishConfig();
        }

        public void Dispose()
        {
            Directory.Delete(Settings.ConfigHelper.ReadonlySettings.GetLocalAppFolderPath(), true);
        }
    }
}
