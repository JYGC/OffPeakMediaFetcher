using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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

        private static void __ImportChannels() // Test this
        {
            OPMF.SiteAdapter.ISiteChannelFinder siteAdapter = null;
            List<OPMF.Entities.IChannel> channels = null;
            try
            {
                siteAdapter = new OPMF.SiteAdapter.Youtube.YoutubeChannelFinder();
                channels = siteAdapter.ImportChannels();

                Console.WriteLine("saving channels to database");
                OPMF.Database.DatabaseAuxillary.RemoveDuplicateIds(channels);
                OPMF.Database.DatabaseAdapter.AccessDbAdapter(dbAdapter =>
                {
                    dbAdapter.YoutubeChannelDbCollection.InsertOrUpdate(channels);
                });
            }
            catch (Exception e)
            {
                OPMF.Logging.Logger.GetCurrent().LogEntry(new OPMF.Entities.OPMFError(e)
                {
                    Variables = new Dictionary<string, object>
                    {
                        { "channels", channels }
                    }
                });
            }
        }

        private static void __ImportChannels(string filePath)
        {
            string opml = null;
            IEnumerable<OPMF.Entities.IChannel> channels = null;
            try
            {
                opml = File.ReadAllText(filePath);
                OPMF.SiteAdapter.Youtube.RssChannelImporter youtubeChannelImporter = new OPMF.SiteAdapter.Youtube.RssChannelImporter(opml);
                channels = youtubeChannelImporter.ImportChannels();
                OPMF.Database.DatabaseAdapter.AccessDbAdapter(dbAdapter =>
                {
                    dbAdapter.YoutubeChannelDbCollection.InsertOrUpdate(channels);
                });
            }
            catch (Exception e)
            {
                OPMF.Logging.Logger.GetCurrent().LogEntry(new OPMF.Entities.OPMFError(e)
                {
                    Variables = new Dictionary<string, object>
                    {
                        { "opml", opml },
                        { "channels", channels }
                    }
                });
            }
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
                        __ImportChannels(openFileDialog.FileName);
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
