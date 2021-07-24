using System;
using System.IO;

namespace OPMF.Tests
{
    public class SetupFixture : IDisposable
    {
        public SetupFixture()
        {
            OSCompat.EnvironmentHelper.EstablishEnvironment();
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
