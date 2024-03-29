﻿using System;
using System.Windows;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Media.Imaging;

namespace FetcherManager.Commons
{
    /// <summary>
    /// Interaction logic for MetadataBrowser.xaml
    /// </summary>
    public partial class UCVideoBrowser : UserControl
    {
        private const string __offPeakMediaFetcherEXE = @"OffPeakMediaFetcher.exe";
        private const string __offPeakMediaFetcherArgsScaffold = "videos {0}";

        private ObservableCollection<OPMF.Entities.IMetadataChannel> __metadataChannels = new ObservableCollection<OPMF.Entities.IMetadataChannel>();

        public Func<int, int, IEnumerable<OPMF.Entities.IMetadataChannel>> GetMetadataChannels { get; set; }
        public Func<IEnumerable<OPMF.Entities.IMetadataChannel>, (IEnumerable<OPMF.Entities.IMetadataChannel>, IEnumerable<OPMF.Entities.IMetadataChannel>)> SplitFromStatus { get; set; }
        public Action<IEnumerable<OPMF.Entities.IMetadataChannel>> SaveMetadataChanges { get; set; }
        public OPMF.Entities.IMetadataChannel SelectedMetadata
        {
            get
            {
                return __metadataChannels[dg_Videos.SelectedIndex];
            }
        }
        public Button Btn_GetVideos
        {
            get
            {
                return btn_GetVideos;
            }
        }

        public UCVideoBrowser()
        {
            InitializeComponent();
        }

        private void __btn_GetVideos_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                __skip = 0;
                __metadataChannels = new ObservableCollection<OPMF.Entities.IMetadataChannel>(GetMetadataChannels(__skip, __pageSize));
                dg_Videos.ItemsSource = __metadataChannels;
                dg_Videos.SelectedIndex = 0;
                btn_Save.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                OPMF.TextLogging.TextLog.GetCurrent().LogEntry(ex.StackTrace);
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void __btn_Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (__metadataChannels.Count == 0) return;
                (IEnumerable<OPMF.Entities.IMetadataChannel>, IEnumerable<OPMF.Entities.IMetadataChannel>) metadatas = SplitFromStatus(__metadataChannels);
                IEnumerable<OPMF.Entities.IMetadataChannel> notNewMetadataChannels = metadatas.Item2;
                SaveMetadataChanges(notNewMetadataChannels.Where(
                    metadataChannel => !metadataChannel.Metadata.IsBeingDownloaded // prevent override of metadata status when it has been downloaded
                ));
                __metadataChannels = new ObservableCollection<OPMF.Entities.IMetadataChannel>(GetMetadataChannels(__skip, __pageSize));
                dg_Videos.ItemsSource = __metadataChannels; // reload dg_VideoInfo
            }
            catch (Exception ex)
            {
                OPMF.TextLogging.TextLog.GetCurrent().LogEntry(ex.StackTrace);
                MessageBox.Show(ex.Message, "Error");
            }
        }

        #region Paging
        private const int __pageSize = 15;
        private int __skip = 0;

        public bool DisablePaging
        {
            set
            {
                dp_Paging.Visibility = value ? Visibility.Hidden : Visibility.Visible;
            }
        }

        private void __btn_Back_Click(object sender, RoutedEventArgs e)
        {
            __skip = __skip - __pageSize;
            __skip = __skip < 0 ? 0 : __skip;
            __metadataChannels = new ObservableCollection<OPMF.Entities.IMetadataChannel>(GetMetadataChannels(__skip, __pageSize));
            dg_Videos.ItemsSource = __metadataChannels;
            dg_Videos.SelectedIndex = 0;
        }

        private void __btn_Forward_Click(object sender, RoutedEventArgs e)
        {
            if (__metadataChannels.Count() < __pageSize) return;
            __skip += __pageSize;
            __metadataChannels = new ObservableCollection<OPMF.Entities.IMetadataChannel>(GetMetadataChannels(__skip, __pageSize));
            dg_Videos.ItemsSource = __metadataChannels;
            dg_Videos.SelectedIndex = 0;
        }
        #endregion

        private void __dg_Videos_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {
                if (dg_Videos.SelectedItem is OPMF.Entities.IMetadataChannel selectedMetadataChannel) // prevent crashing when selected item is deleted
                {
                    lbl_Title.Content = selectedMetadataChannel.Metadata.Title;
                    lbl_Channel.Content = selectedMetadataChannel.Channel.Name;
                    hl_Url.NavigateUri = new Uri(selectedMetadataChannel.Metadata.Url);
                    txt_UrlTextBlock.Text = selectedMetadataChannel.Metadata.Url;
                    img_Thumbnail.Source = (selectedMetadataChannel.Metadata.Thumbnail.Url != null) ? new BitmapImage(new Uri(selectedMetadataChannel.Metadata.Thumbnail.Url, UriKind.Absolute)) : null;
                    img_Thumbnail.Width = selectedMetadataChannel.Metadata.Thumbnail.Width;
                    img_Thumbnail.Height = selectedMetadataChannel.Metadata.Thumbnail.Height;
                    lbl_Description.Text = selectedMetadataChannel.Metadata.Description;
                    btn_DownloadNow.Visibility = selectedMetadataChannel.Metadata.IsBeingDownloaded ? Visibility.Collapsed : Visibility.Visible;
                    btn_RemoveIsBeingDownloaded.Visibility = lbl_IsBeingDownloaded.Visibility = selectedMetadataChannel.Metadata.IsBeingDownloaded ? Visibility.Visible : Visibility.Hidden;
                }
                else
                {
                    lbl_Title.Content = null;
                    lbl_Channel.Content = null;
                    hl_Url.NavigateUri = null;
                    txt_UrlTextBlock.Text = null;
                    img_Thumbnail.Source = null;
                    img_Thumbnail.Width = 0;
                    img_Thumbnail.Height = 0;
                    lbl_Description.Text = null;
                    btn_DownloadNow.Visibility = Visibility.Hidden;
                    btn_RemoveIsBeingDownloaded.Visibility = lbl_IsBeingDownloaded.Visibility = Visibility.Hidden;
                }
            }
            catch (Exception ex)
            {
                OPMF.TextLogging.TextLog.GetCurrent().LogEntry(ex.StackTrace);
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void __hl_Url_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo(OPMF.Settings.ConfigHelper.Config.WebBrowserPath, e.Uri.AbsoluteUri));
                e.Handled = true;
            }
            catch (Exception ex)
            {
                OPMF.TextLogging.TextLog.GetCurrent().LogEntry(ex.StackTrace);
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void __btn_DownloadNow_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SelectedMetadata.Metadata.IsBeingDownloaded = true;
                SelectedMetadata.Metadata.Status = OPMF.Entities.MetadataStatus.ToDownload;
                __dg_Videos_SelectedCellsChanged(null, null);
                Data.MetadataManager __metadataManager = new Data.MetadataManager();
                __metadataManager.SaveMetadataChanges(new List<OPMF.Entities.IMetadataChannel> { SelectedMetadata });

                Process process = new Process();
                process.StartInfo.FileName = __offPeakMediaFetcherEXE;
                process.StartInfo.Arguments = string.Format(__offPeakMediaFetcherArgsScaffold, SelectedMetadata.Metadata.SiteId);
                process.Start();
            }
            catch (Exception ex)
            {
                OPMF.TextLogging.TextLog.GetCurrent().LogEntry(ex.StackTrace);
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void __btn_RemoveIsBeingDownloaded_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SelectedMetadata.Metadata.IsBeingDownloaded = false;
                __dg_Videos_SelectedCellsChanged(null, null);
                OPMF.Database.DatabaseAdapter.AccessDbAdapter(dbconn => dbconn.YoutubeMetadataDbCollection.UpdateIsBeingProcessed(
                    new List<OPMF.Entities.IMetadata> { SelectedMetadata.Metadata }
                ));
            }
            catch (Exception ex)
            {
                OPMF.TextLogging.TextLog.GetCurrent().LogEntry(ex.StackTrace);
                MessageBox.Show(ex.Message, "Error");
            }
        }
    }
}
