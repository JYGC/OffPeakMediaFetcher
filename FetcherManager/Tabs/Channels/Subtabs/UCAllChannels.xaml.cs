﻿using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using System;
using System.Linq;
using System.Windows.Media.Imaging;
using System.Collections.Generic;

namespace FetcherManager.Tabs.Channels.Subtabs
{
    /// <summary>
    /// Interaction logic for UCAllChannels.xaml
    /// </summary>
    public partial class UCAllChannels : UserControl
    {
        private ObservableCollection<OPMF.Entities.IPropertyChangeChannel> __channels;

        public RoutedCommand BlacklistChannel { get; set; } = new RoutedCommand();
        public RoutedCommand UnblacklistChannel { get; set; } = new RoutedCommand();

        public UCAllChannels()
        {
            InitializeComponent();

            __InitializeKeyBindings();
        }

        private void __InitializeKeyBindings()
        {
            cb_Blacklist.Command = BlacklistChannel;
            BlacklistChannel.InputGestures.Add(new KeyGesture(Key.F1));
            cb_Unblacklist.Command = UnblacklistChannel;
            UnblacklistChannel.InputGestures.Add(new KeyGesture(Key.F2));
        }

        private static IEnumerable<OPMF.Entities.IPropertyChangeChannel> __GetAllChannelsFromDB()
        {
            IEnumerable<OPMF.Entities.IPropertyChangeChannel> channels = new OPMF.Entities.IPropertyChangeChannel[] { };

            OPMF.Database.DatabaseAdapter.AccessDbAdapter(dbAdapter =>
            {
                IEnumerable<OPMF.Entities.IChannel> rawChannels = dbAdapter.YoutubeChannelDbCollection.GetAll();
                foreach (OPMF.Entities.IChannel rawChannel in rawChannels)
                {
                    channels = channels.Concat(new OPMF.Entities.IPropertyChangeChannel[]
                    {
                        new OPMF.Entities.PropertyChangeChannel(rawChannel)
                    });
                }
            });

            return channels;
        }

        private void __btn_GetAllChannels_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                __channels = new ObservableCollection<OPMF.Entities.IPropertyChangeChannel>(__GetAllChannelsFromDB().OrderBy(c => c.Name));

                LoadingDialog loadingDialog = new LoadingDialog("Loading " + __channels.Count.ToString() + " channels entries", () =>
                {
                    dg_Channels.ItemsSource = __channels;
                    dg_Channels.SelectedIndex = 0;
                    btn_Save.Visibility = Visibility.Visible;
                });
                loadingDialog.Show();
            }
            catch (Exception ex)
            {
                OPMF.TextLogging.TextLog.GetCurrent().LogEntry(e.ToString());
                MessageBox.Show(ex.Message, "Error");
            }
        }

        public static void __UpdateChannelSettingsInDB(IEnumerable<OPMF.Entities.IChannel> channels)
        {
            OPMF.Database.DatabaseAdapter.AccessDbAdapter(dbAdapter =>
            {
                dbAdapter.YoutubeChannelDbCollection.UpdateBlackListStatus(channels);
            });
        }

        private void __btn_Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (__channels.Count > 0)
                {
                    LoadingDialog loadingDialog = new LoadingDialog("Saving channel selection...", () =>
                    {
                        __UpdateChannelSettingsInDB(__channels);
                    });
                    loadingDialog.Show();
                }
            }
            catch (Exception ex)
            {
                OPMF.TextLogging.TextLog.GetCurrent().LogEntry(e.ToString());
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void __sv_Channels_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            try
            {
                sv_Channels.ScrollToVerticalOffset(sv_Channels.VerticalOffset - e.Delta);
            }
            catch (Exception ex)
            {
                OPMF.TextLogging.TextLog.GetCurrent().LogEntry(e.ToString());
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void __dg_Channels_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {
                if (dg_Channels.SelectedItem is OPMF.Entities.IPropertyChangeChannel selectedChannel) // IDE0019
                {
                    lbl_ChannelName.Content = selectedChannel.Name;
                    hl_Url.NavigateUri = new Uri(selectedChannel.Url);
                    txt_UrlTextBlock.Text = selectedChannel.Url;
                    img_Thumbnail.Source = (selectedChannel.Thumbnail.Url != null) ? new BitmapImage(new Uri(selectedChannel.Thumbnail.Url, UriKind.Absolute)) : null;
                    img_Thumbnail.Width = selectedChannel.Thumbnail.Width;
                    img_Thumbnail.Height = selectedChannel.Thumbnail.Height;
                    lbl_LastCheckedOut.Content = selectedChannel.LastCheckedOut;
                    lbl_LastActivityDate.Content = selectedChannel.LastActivityDate;
                    lbl_Status.Content = selectedChannel.NotFound ? "Channel Not Found" : "Channel Exists";
                    lbl_Description.Content = selectedChannel.Description;
                }
            }
            catch (Exception ex)
            {
                OPMF.TextLogging.TextLog.GetCurrent().LogEntry(e.ToString());
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void __cb_Blacklist_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            __channels[dg_Channels.SelectedIndex].BlackListed = true;
        }

        private void __cb_Unblacklist_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            __channels[dg_Channels.SelectedIndex].BlackListed = false;
        }

        private void __hl_Url_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(OPMF.Settings.ConfigHelper.Config.WebBrowserPath, e.Uri.AbsoluteUri));
                e.Handled = true;
            }
            catch (Exception ex)
            {
                OPMF.TextLogging.TextLog.GetCurrent().LogEntry(e.ToString());
                MessageBox.Show(ex.Message, "Error");
            }
        }
    }
}
