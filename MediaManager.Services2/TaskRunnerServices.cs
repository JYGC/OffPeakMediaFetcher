using OPMF.Entities;
using System.Diagnostics;

namespace MediaManager.Services2
{
    public interface ITaskRunnerServices
    {
        void DownloadOneVideo(Metadata metadata);
    }
    public class TaskRunnerServices : ITaskRunnerServices
    {
        private const string _taskRunnerExeName = @"OffPeakMediaFetcher.exe";

        public void DownloadOneVideo(Metadata metadata)
        {
            const string taskRunnerArgumentScaffold = "videos {0}";

            Process process = new Process();
            process.StartInfo.FileName = _taskRunnerExeName;
            process.StartInfo.Arguments = string.Format(taskRunnerArgumentScaffold, metadata.SiteId);
            process.Start();
        }
    }
}
