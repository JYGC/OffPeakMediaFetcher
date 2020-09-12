using Microsoft.Win32;
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
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                LoadingDialog loadingDialog = new LoadingDialog("Importing channels...", () =>
                {
                    OPMF.Actions.SiteDownload.ImportChannels(openFileDialog.FileName);
                });
            }
        }
    }
}
