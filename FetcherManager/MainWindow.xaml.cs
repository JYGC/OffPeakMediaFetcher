using System.Windows;

namespace FetcherManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            try
            {
                OPMF.Settings.ConfigHelper.InitReadonlySettings();
                OPMF.Actions.FolderSetup.EstablishFolders();
                OPMF.Settings.ConfigHelper.EstablishConfig();

                InitializeComponent();
            }
            catch (System.Exception e)
            {
                OPMF.TextLogging.TextLog.GetCurrent().LogEntry(e.ToString());
                MessageBox.Show(e.Message, "Error");
            }
        }
    }
}
