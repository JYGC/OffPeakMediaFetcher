using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows;
using System;
using System.Linq;

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

        private void __btn_GetAllChannels_Click(object sender, RoutedEventArgs e)
        {
            __channels = new ObservableCollection<OPMF.Entities.IPropertyChangeChannel>(OPMF.Actions.ChannelManagement.GetAllChannels().OrderBy(c => c.Name));

            LoadingDialog loadingDialog = new LoadingDialog("Loading " + __channels.Count.ToString() + " channels entries", () =>
            {
                dg_Channels.ItemsSource = __channels;
                dg_Channels.SelectedIndex = 0;
                btn_Save.Visibility = Visibility.Visible;
            });
            loadingDialog.Show();
        }

        private void __btn_Save_Click(object sender, RoutedEventArgs e)
        {
            if (__channels.Count > 0)
            {
                LoadingDialog loadingDialog = new LoadingDialog("Saving channel selection...", () =>
                {
                    OPMF.Actions.ChannelManagement.UpdateChannelSettings(__channels);
                });
                loadingDialog.Show();
            }
        }

        private void __sv_Channels_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            sv_Channels.ScrollToVerticalOffset(sv_Channels.VerticalOffset - e.Delta);
        }

        private void __dg_Channels_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            OPMF.Entities.IPropertyChangeChannel selectedChannel = dg_Channels.SelectedItem as OPMF.Entities.IPropertyChangeChannel;
            if (selectedChannel != null)
            {
                lbl_ChannelName.Content = selectedChannel.Name;
                hl_Url.NavigateUri = new Uri(selectedChannel.Url);
                txt_UrlTextBlock.Text = selectedChannel.Url;
                lbl_LastCheckedOut.Content = selectedChannel.LastCheckedOut;
                lbl_LastActivityDate.Content = selectedChannel.LastActivityDate;
                lbl_Status.Content = selectedChannel.NotFound ? "Channel Not Found" : "Channel Exists";
                lbl_Description.Content = selectedChannel.Description;
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
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(OPMF.Settings.ConfigHelper.Config.WebBrowserPath, e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }
}
