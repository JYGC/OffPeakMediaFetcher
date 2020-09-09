using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;

namespace FetcherManager
{
    /// <summary>
    /// Interaction logic for LoadingDialog.xaml
    /// </summary>
    public partial class LoadingDialog : Window
    {
        private Action __WorkAction;

        public LoadingDialog(string message, Action WorkAction)
        {
            __WorkAction = WorkAction;

            InitializeComponent();

            _message.Content = message;
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            Focus();
            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += WorkToDo;

            worker.RunWorkerAsync();
            worker.RunWorkerCompleted += (object sender, RunWorkerCompletedEventArgs e) =>
            {
                Close();
            };
        }

        public void WorkToDo(object sender, DoWorkEventArgs e)
        {
           Dispatcher.BeginInvoke(new ThreadStart(() =>
           {
               __WorkAction();
           }));
        }
    }
}
