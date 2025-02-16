using System.Windows;
using MediaManager.Services2;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;

namespace MediaManagerUI
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
                InitializeComponent();

                var serviceCollection = new ServiceCollection();

                serviceCollection.AddScoped<ITaskRunnerServices, TaskRunnerServices>();
                serviceCollection.AddScoped<IChannelServices, ChannelServices>();
                serviceCollection.AddScoped<IMetadataServices, MetadataServices>();
                serviceCollection.AddScoped<IChannelMetadataServices, ChannelMetadataServices>();
                serviceCollection.AddScoped<ILogServices, LogServices>();

                serviceCollection.AddMudServices();
                serviceCollection.AddWpfBlazorWebView();

                Resources.Add("services", serviceCollection.BuildServiceProvider());
            }
            catch (Exception e)
            {
                OPMF.TextLogging.TextLog.GetCurrent().LogEntry(e.ToString());
                MessageBox.Show(e.Message, "Error");
            }
        }
    }
}