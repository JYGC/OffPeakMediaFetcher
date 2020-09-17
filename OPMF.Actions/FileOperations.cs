using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OPMF.Actions
{
    public static class FileOperations
    {
        public static void MoveAllInFolder(string srcFolderPath, string dstFolderPath, string[] fileExtensions)
        {
            IEnumerable<string> srcFilepaths = new string[] { };

            foreach (string fileExtension in fileExtensions)
            {
                srcFilepaths = srcFilepaths.Concat(Directory.GetFiles(srcFolderPath).Where(f => f.EndsWith("." + fileExtension)));
            }

            foreach (string srcFile in srcFilepaths)
            {
                FileInfo srcFileInfo = new FileInfo(srcFile);
                if (new FileInfo(Path.Join(dstFolderPath, srcFileInfo.Name)).Exists == false)
                {
                    srcFileInfo.MoveTo(Path.Join(dstFolderPath, srcFileInfo.Name));
                }
            }
        }
    }
}
