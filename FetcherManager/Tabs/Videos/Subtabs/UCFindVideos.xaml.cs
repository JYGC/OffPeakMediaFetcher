using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FetcherManager.Tabs.Videos.Subtabs
{
    /// <summary>
    /// Interaction logic for UCFindVideos.xaml
    /// </summary>
    public partial class UCFindVideos : UserControl
    {
        public RoutedCommand IgnoreMetadata { get; set; } = new RoutedCommand();
        public RoutedCommand ToDownloadMetadata { get; set; } = new RoutedCommand();
        public RoutedCommand BackToNewMetadata { get; set; } = new RoutedCommand();
        public RoutedCommand SetToWaitMetadata { get; set; } = new RoutedCommand();

        public UCFindVideos()
        {
            InitializeComponent();
            __PrepareChildUserControls();
            __InitializeKeyBindings();
        }

        private void __PrepareChildUserControls()
        {
            uc_VideoBrowser.Btn_GetVideos.Content = "Find";
            uc_VideoBrowser.Btn_GetVideos.Visibility = Visibility.Hidden;
            uc_VideoBrowser.GetMetadataChannels = () =>
            {
                if (string.IsNullOrWhiteSpace(txt_FindByChannelName.Text))
                {
                    return OPMF.Actions.MetadataManagement.GetByTitleContainingWord(
                        txt_FindByVideoTitle.Text).OrderByDescending(c => c.Metadata.PublishedAt);
                }
                return OPMF.Actions.MetadataManagement.GetByChannelAndTitleContainingWord(
                    txt_FindByChannelName.Text, txt_FindByVideoTitle.Text).OrderByDescending(c => c.Metadata.PublishedAt);
            };
            uc_VideoBrowser.SplitFromStatus = (metadataChannels) => (metadataChannels, metadataChannels);
            uc_VideoBrowser.SaveMetadataChanges = (notStatusMetadataChannels) => OPMF.Actions.MetadataManagement.SaveMetadataChanges(notStatusMetadataChannels);
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

        private void __txt_FindBy_TextChanged(object sender, TextChangedEventArgs e)
        {
            uc_VideoBrowser.Btn_GetVideos.Visibility = (string.IsNullOrWhiteSpace(txt_FindByChannelName.Text) && string.IsNullOrWhiteSpace(txt_FindByVideoTitle.Text)) ? Visibility.Hidden : Visibility.Visible;
        }
    }
}
