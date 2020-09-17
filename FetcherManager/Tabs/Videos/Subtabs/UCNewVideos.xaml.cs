using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private ObservableCollection<OPMF.Entities.IMetadataChannel> __metadataChannels = new ObservableCollection<OPMF.Entities.IMetadataChannel>();

        public RoutedCommand IgnoreMetadata { get; set; } = new RoutedCommand();
        public RoutedCommand ToDownloadMetadata { get; set; } = new RoutedCommand();
        public RoutedCommand BackToNewMetadata { get; set; } = new RoutedCommand();

        public UCNewVideos()
        {
            InitializeComponent();

            __InitializeKeyBindings();
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

        private void __btn_GetNewVideos_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            __metadataChannels = new ObservableCollection<OPMF.Entities.IMetadataChannel>(OPMF.Actions.MetadataManagement.GetMetadataChannels().OrderBy(c => c.Channel.Name));

            LoadingDialog loadingDialog = new LoadingDialog("Loading " + __metadataChannels.Count.ToString() + " video entries...", () =>
            {
                dg_VideoInfo.ItemsSource = __metadataChannels;
                dg_VideoInfo.SelectedIndex = 0;
                btn_Save.Visibility = System.Windows.Visibility.Visible;
            });
            loadingDialog.Show();
        }

        private void __btn_Save_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (__metadataChannels.Count > 0)
            {
                LoadingDialog loadingDialog = new LoadingDialog("Saving video selection...", () =>
                {
                    (IEnumerable<OPMF.Entities.IMetadataChannel>, IEnumerable<OPMF.Entities.IMetadataChannel>) metadatas = OPMF.Actions.MetadataManagement.SplitFromNew(__metadataChannels);
                    IEnumerable<OPMF.Entities.IMetadataChannel> notNewMetadataChannels = metadatas.Item2;
                    __metadataChannels = new ObservableCollection<OPMF.Entities.IMetadataChannel>(metadatas.Item1);
                    OPMF.Actions.MetadataManagement.SaveMetadataChanges(notNewMetadataChannels);
                    dg_VideoInfo.ItemsSource = __metadataChannels; // reload dg_VideoInfo
                });
                loadingDialog.Show();
            }
        }

        private void __dg_VideoInfo_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            OPMF.Entities.IMetadataChannel selectedMetadataChannel = dg_VideoInfo.SelectedItem as OPMF.Entities.IMetadataChannel;
            if (selectedMetadataChannel != null) // prevent crashing when selected item is deleted
            {
                lbl_Title.Content = selectedMetadataChannel.Metadata.Title;
                lbl_Channel.Content = selectedMetadataChannel.Channel.Name;
                hl_Url.NavigateUri = new Uri(selectedMetadataChannel.Metadata.Url);
                txt_UrlTextBlock.Text = selectedMetadataChannel.Metadata.Url;
                lbl_Description.Text = selectedMetadataChannel.Metadata.Description;
            }
        }

        private void __sv_VideoInfo_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            sv_VideoInfo.ScrollToVerticalOffset(sv_VideoInfo.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private void __cb_Ignore_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            __metadataChannels[dg_VideoInfo.SelectedIndex].Metadata.Status = OPMF.Entities.MetadataStatus.Ignore;
        }

        private void __cb_ToDownload_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            __metadataChannels[dg_VideoInfo.SelectedIndex].Metadata.Status = OPMF.Entities.MetadataStatus.ToDownload;
        }

        private void __cb_BackToNew_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            __metadataChannels[dg_VideoInfo.SelectedIndex].Metadata.Status = OPMF.Entities.MetadataStatus.New;
        }

        private void __hl_Url_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(OPMF.Settings.ConfigHelper.Config.WebBrowserPath, e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
