using System;
using System.Collections.Generic;
using System.Text;

namespace OPMF.OSEnvironment
{
    public interface IOSEnvironment
    {
        string GetUserLocalAppFolderPath();

        string GetUserTempFolderPath();

        string GetProgramFolderPath();
    }
}
