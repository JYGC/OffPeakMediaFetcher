using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace FetcherManager.Tabs.Videos.Subtabs
{
    /// <summary>
    /// Interaction logic for NewVideos.xaml
    /// </summary>
    public partial class UCNewVideos : UserControl
    {
        public RoutedCommand IgnoreMetadata { get; set; } = new RoutedCommand();
        public RoutedCommand ToDownloadMetadata { get; set; } = new RoutedCommand();
        public RoutedCommand BackToNewMetadata { get; set; } = new RoutedCommand();
        public RoutedCommand SetToWaitMetadata { get; set; } = new RoutedCommand();

        public UCNewVideos()
        {
            InitializeComponent();
            __PrepareChildUserControls();
            __InitializeKeyBindings();
        }

        private void __PrepareChildUserControls()
        {
            uc_VideoBrowser.Btn_GetVideos.Content = "Get New Videos";
            Data.MetadataManager __metadataManager = new Data.MetadataManager();
            uc_VideoBrowser.GetMetadataChannels = (skip, pageSize) => __metadataManager.GetNew(skip, pageSize).OrderBy(c => c.Channel.Name);
            uc_VideoBrowser.SplitFromStatus = (metadataChannels) => __metadataManager.SplitFromStatus(metadataChannels, OPMF.Entities.MetadataStatus.New);
            uc_VideoBrowser.SaveMetadataChanges = (notStatusMetadataChannels) => __metadataManager.SaveMetadataChanges(notStatusMetadataChannels);
        }

        private void __InitializeKeyBindings()
        {
            cb_Ignore.Command = IgnoreMetadata;
            IgnoreMetadata.InputGestures.Add(new KeyGesture(Key.F1));
            cb_ToDownload.Command = ToDownloadMetadata;
            ToDownloadMetadata.InputGestures.Add(new KeyGesture(Key.F2));
            cb_BackToNew.Command = BackToNewMetadata;
            BackToNewMetadata.InputGestures.Add(new KeyGesture(Key.F3));
            cb_SetToWait.Command = SetToWaitMetadata;
            SetToWaitMetadata.InputGestures.Add(new KeyGesture(Key.F4));
        }

        private void __cb_Ignore_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (uc_VideoBrowser.SelectedMetadata.Metadata.IsBeingDownloaded)
            {
                return;
            }
            uc_VideoBrowser.SelectedMetadata.Metadata.Status = OPMF.Entities.MetadataStatus.Ignore;
        }

        private void __cb_ToDownload_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (uc_VideoBrowser.SelectedMetadata.Metadata.IsBeingDownloaded)
            {
                return;
            }
            uc_VideoBrowser.SelectedMetadata.Metadata.Status = OPMF.Entities.MetadataStatus.ToDownload;
        }

        private void __cb_BackToNew_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (uc_VideoBrowser.SelectedMetadata.Metadata.IsBeingDownloaded)
            {
                return;
            }
            uc_VideoBrowser.SelectedMetadata.Metadata.Status = OPMF.Entities.MetadataStatus.New;
        }

        private void __cb_SetToWait_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (uc_VideoBrowser.SelectedMetadata.Metadata.IsBeingDownloaded)
            {
                return;
            }
            uc_VideoBrowser.SelectedMetadata.Metadata.Status = OPMF.Entities.MetadataStatus.Wait;
        }
    }
}
