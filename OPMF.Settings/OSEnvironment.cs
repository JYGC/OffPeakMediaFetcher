using System;
using System.IO;
using System.Reflection;

namespace OPMF.Settings
{
    class OSEnvironment
    {
        public static string GetExecDirectory()
        {
            return new FileInfo((new Uri(Assembly.GetEntryAssembly().GetName().CodeBase)).AbsolutePath).DirectoryName;
        }

        public static string GetUserLocalAppDirectory()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        }
    }
}
