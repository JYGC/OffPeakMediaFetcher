using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace FetcherManager.Tabs.Videos.Subtabs
{
    /// <summary>
    /// Interaction logic for UCDownloadedVideos.xaml
    /// </summary>
    public partial class UCDownloadedVideos : UserControl
    {
        private int __resultsLimit = 1000;

        public RoutedCommand ToDownloadMetadata { get; set; } = new RoutedCommand();
        public RoutedCommand BackToDownloadedMetadata { get; set; } = new RoutedCommand();

        public UCDownloadedVideos()
        {
            InitializeComponent();
            __PrepareChildUserControls();
            __InitializeKeyBindings();
        }

        private void __PrepareChildUserControls()
        {
            uc_VideoBrowser.Btn_GetVideos.Content = "Get Downloaded";
            uc_VideoBrowser.GetMetadataChannels = () => OPMF.Actions.MetadataManagement.GetDownloaded().OrderByDescending(c => c.Metadata.PublishedAt).Take(__resultsLimit);
            uc_VideoBrowser.SplitFromStatus = (metadataChannels) => OPMF.Actions.MetadataManagement.SplitFromStatus(metadataChannels, OPMF.Entities.MetadataStatus.Downloaded);
            uc_VideoBrowser.SaveMetadataChanges = (notStatusMetadataChannels) => OPMF.Actions.MetadataManagement.SaveMetadataChanges(notStatusMetadataChannels);
        }

        private void __InitializeKeyBindings()
        {
            cb_ToDownload.Command = ToDownloadMetadata;
            ToDownloadMetadata.InputGestures.Add(new KeyGesture(Key.F1));
            cb_BackToDownloaded.Command = BackToDownloadedMetadata;
            BackToDownloadedMetadata.InputGestures.Add(new KeyGesture(Key.F2));
        }
    }
}
