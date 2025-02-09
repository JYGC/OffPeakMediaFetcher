using OPMF.Entities;
using System.Diagnostics;

namespace MediaManager.Services
{
    public interface ITaskRunnerServices
    {
        void DownloadOneVideo(IMetadata metadata);
    }
    public class TaskRunnerServices : ITaskRunnerServices
    {
        private const string _taskRunnerExeName = @"OffPeakMediaFetcher.exe";

        public void DownloadOneVideo(IMetadata metadata)
        {
            const string taskRunnerArgumentScaffold = "videos {0}";

            Process process = new Process();
            process.StartInfo.FileName = _taskRunnerExeName;
            process.StartInfo.Arguments = string.Format(taskRunnerArgumentScaffold, metadata.SiteId);
            process.Start();
        }
    }
}
