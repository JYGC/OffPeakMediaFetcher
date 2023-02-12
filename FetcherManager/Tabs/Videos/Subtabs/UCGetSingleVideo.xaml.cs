using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FetcherManager.Tabs.Videos.Subtabs
{
    /// <summary>
    /// Interaction logic for UCGetSingleVideo.xaml
    /// </summary>
    public partial class UCGetSingleVideo : UserControl
    {
        public RoutedCommand IgnoreMetadata { get; set; } = new RoutedCommand();
        public RoutedCommand ToDownloadMetadata { get; set; } = new RoutedCommand();
        public RoutedCommand BackToNewMetadata { get; set; } = new RoutedCommand();
        public RoutedCommand SetToWaitMetadata { get; set; } = new RoutedCommand();

        public UCGetSingleVideo()
        {
            InitializeComponent();
            __PrepareChildUserControls();
            __InitializeKeyBindings();
        }

        private void __PrepareChildUserControls()
        {
            uc_VideoBrowser.Btn_GetVideos.Content = "Get Video";
            uc_VideoBrowser.Btn_GetVideos.Visibility = Visibility.Hidden;
            uc_VideoBrowser.GetMetadataChannels = () =>
            {
                OPMF.SiteAdapter.ISiteVideoMetadataGetter siteVideoGetter = new OPMF.SiteAdapter.Youtube.YoutubeVideoMetadataGetter(); // Replace when adding other platforms
                string siteId = siteVideoGetter.GetSiteIdFromURL(txt_EnterVideoURL.Text);
                (OPMF.Entities.IMetadata, OPMF.Entities.IChannel) videoWithChannel = (null, null);
                OPMF.Database.DatabaseAdapter.AccessDbAdapter((dbAdapter) =>
                {
                    videoWithChannel.Item1 = dbAdapter.YoutubeMetadataDbCollection.GetBySiteId(siteId);
                    if (videoWithChannel.Item1 == null)
                    {
                        videoWithChannel = siteVideoGetter.GetVideoByURL(siteId);
                        dbAdapter.YoutubeMetadataDbCollection.InsertNew(new List<OPMF.Entities.IMetadata> { videoWithChannel.Item1 });
                        dbAdapter.YoutubeChannelDbCollection.InsertOrUpdate(new List<OPMF.Entities.IChannel> { videoWithChannel.Item2 });
                    }
                    else
                    {
                        videoWithChannel.Item2 = dbAdapter.YoutubeChannelDbCollection.GetBySiteId(videoWithChannel.Item1.ChannelSiteId);
                    }
                });
                return new OPMF.Entities.IMetadataChannel[] { new OPMF.Entities.MetadataChannel
                {
                    Metadata = new OPMF.Entities.PropertyChangedMetadata(videoWithChannel.Item1),
                    Channel = videoWithChannel.Item2
                }};
            };
            uc_VideoBrowser.SplitFromStatus = (metadataChannels) => (metadataChannels, metadataChannels);
            Data.MetadataManager __metadataManager = new Data.MetadataManager();
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

        private void __txt_EnterVideoURL_TextChanged(object sender, TextChangedEventArgs e)
        {
            uc_VideoBrowser.Btn_GetVideos.Visibility = string.IsNullOrWhiteSpace(txt_EnterVideoURL.Text) ? Visibility.Hidden : Visibility.Visible;
        }
    }
}
