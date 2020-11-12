using System;
using System.IO;
using System.Reflection;

namespace OPMF.OSEnvironment.Windows
{
    public class WinEnvironment : IOSEnvironment
    {
        public string GetUserLocalAppFolderPath()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        }

        public string GetUserTempFolderPath()
        {
            return Path.GetTempPath();
        }

        public string GetProgramFolderPath()
        {
            string programFolderPath = null;

            Uri location = new Uri(Assembly.GetEntryAssembly().GetName().CodeBase);
            FileInfo execFileInfo = new FileInfo(location.AbsolutePath);
            programFolderPath = execFileInfo.Directory.FullName;

            return programFolderPath;
        }
    }
}
