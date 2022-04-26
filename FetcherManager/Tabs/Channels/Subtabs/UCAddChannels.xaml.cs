using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace FetcherManager.Tabs.Channels.Subtabs
{
    /// <summary>
    /// Interaction logic for UCAddChannels.xaml
    /// </summary>
    public partial class UCAddChannels : UserControl
    {
        public UCAddChannels()
        {
            InitializeComponent();
            rb_SelectName.IsChecked = true;
        }

        private void __btn_ImportYoutubeXml_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                if (openFileDialog.ShowDialog() == true)
                {
                    LoadingDialog loadingDialog = new LoadingDialog("Importing channels...", () =>
                    {
                        OPMF.Actions.SiteDownload.ImportChannels(openFileDialog.FileName);
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

        private void __btn_SearchChannel_Click(object sender, RoutedEventArgs e)
        {
            OPMF.SiteAdapter.ISiteChannelFinder channelImporter = new OPMF.SiteAdapter.Youtube.YoutubeChannelFinder();
            List<OPMF.Entities.IChannel> channels = rb_SelectName.IsChecked.Value ? channelImporter.FindChannelByName(new string[] { txt_ChannelIdentifier.Text }) : channelImporter.FindChannelById(new string[] { txt_ChannelIdentifier.Text });
            if (channels == null) return;
            foreach (OPMF.Entities.IChannel channel in channels)
            {
                lbl_Res.Content = string.Format("SiteId: {0}\nTitle: {1}\nUrl: {2}\nDescription: {3}\n\n", channel.SiteId, channel.Name, channel.Url, channel.Description);
            }
        }

        private void __rb_SelectName_Checked(object sender, RoutedEventArgs e)
        {
            rb_SelectName.IsChecked = true;
            rb_SelectId.IsChecked = false;
        }

        private void __rb_SelectId_Checked(object sender, RoutedEventArgs e)
        {
            rb_SelectName.IsChecked = false;
            rb_SelectId.IsChecked = true;
        }
    }
}
