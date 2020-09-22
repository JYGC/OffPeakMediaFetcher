using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;

namespace FetcherManager.Tabs.Videos.Subtabs
{
    /// <summary>
    /// Interaction logic for UCToDownload.xaml
    /// </summary>
    public partial class UCToDownload : UserControl
    {
        public RoutedCommand IgnoreMetadata { get; set; } = new RoutedCommand();
        public RoutedCommand ToDownloadMetadata { get; set; } = new RoutedCommand();
        public RoutedCommand BackToNewMetadata { get; set; } = new RoutedCommand();

        public UCToDownload()
        {
            InitializeComponent();
            __PrepareChildUserControls();
            __InitializeKeyBindings();
        }

        private void __PrepareChildUserControls()
        {
            uc_VideoBrowser.Btn_GetVideos.Content = "Get Download Queue";
            uc_VideoBrowser.GetMetadataChannels = () => OPMF.Actions.MetadataManagement.GetToDownload().OrderBy(c => c.Channel.Name);
            uc_VideoBrowser.SplitFromStatus = (metadataChannels) => OPMF.Actions.MetadataManagement.SplitFromStatus(metadataChannels, OPMF.Entities.MetadataStatus.ToDownload);
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
        }

        private void __cb_Ignore_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            uc_VideoBrowser.SelectedMetadata.Metadata.Status = OPMF.Entities.MetadataStatus.Ignore;
        }

        private void __cb_ToDownload_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            uc_VideoBrowser.SelectedMetadata.Metadata.Status = OPMF.Entities.MetadataStatus.ToDownload;
        }

        private void __cb_BackToNew_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            uc_VideoBrowser.SelectedMetadata.Metadata.Status = OPMF.Entities.MetadataStatus.New;
        }
    }
}
