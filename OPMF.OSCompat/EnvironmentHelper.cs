using System;

namespace OPMF.OSCompat
{
    public static class EnvironmentHelper
    {
        private static OSEnvironment.IOSEnvironment __environment;

        public static OSEnvironment.IOSEnvironment Environment
        {
            get
            {
                return __environment;
            }
        }

        public static void EstablishEnvironment()
        {
            __environment = new OSEnvironment.Windows.WinEnvironment();
        }
    }
}
