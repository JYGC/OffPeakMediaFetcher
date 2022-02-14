using Microsoft.Win32;
using System;
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
    }
}
