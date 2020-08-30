using System.IO;

namespace OPMF.Actions
{
    public static class FileOperations
    {
        public static void MoveAllInFolder(string srcFolderPath, string dstFolderPath, string fileExtension)
        {
            string[] srcFilepaths = Directory.GetFiles(srcFolderPath, "*." + fileExtension);
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
