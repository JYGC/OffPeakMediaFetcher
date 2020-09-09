using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            IgnoreMetadata.InputGestures.Add(new KeyGesture(Key.OemMinus));
            cb_ToDownload.Command = ToDownloadMetadata;
            ToDownloadMetadata.InputGestures.Add(new KeyGesture(Key.OemPlus));
            cb_BackToNew.Command = BackToNewMetadata;
            BackToNewMetadata.InputGestures.Add(new KeyGesture(Key.Back));
        }

        private void __btn_GetNewVideos_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            __metadataChannels = new ObservableCollection<OPMF.Entities.IMetadataChannel>(OPMF.Actions.RecordsManagement.GetMetadataChannels());

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
                    (IEnumerable<OPMF.Entities.IMetadataChannel>, IEnumerable<OPMF.Entities.IMetadataChannel>) metadatas = OPMF.Actions.RecordsManagement.SplitFromNew(__metadataChannels);
                    IEnumerable<OPMF.Entities.IMetadataChannel> notNewMetadataChannels = metadatas.Item2;
                    __metadataChannels = new ObservableCollection<OPMF.Entities.IMetadataChannel>(metadatas.Item1);
                    OPMF.Actions.RecordsManagement.SaveMetadataChanges(notNewMetadataChannels);
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
                lbl_Description.Content = selectedMetadataChannel.Metadata.Description;
            }
        }

        private void __sv_VideoInfo_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            sv_VideoInfo.ScrollToVerticalOffset(sv_VideoInfo.VerticalOffset - e.Delta);
            e.Handled = true;
        }

        private void __ignoreBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            __metadataChannels[dg_VideoInfo.SelectedIndex].Metadata.Status = OPMF.Entities.MetadataStatus.Ignore;
        }

        private void __toDownloadBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            __metadataChannels[dg_VideoInfo.SelectedIndex].Metadata.Status = OPMF.Entities.MetadataStatus.ToDownload;
        }

        private void __backToNewCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            __metadataChannels[dg_VideoInfo.SelectedIndex].Metadata.Status = OPMF.Entities.MetadataStatus.New;
        }
    }
}
