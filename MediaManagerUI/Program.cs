namespace MediaManagerUI
{
    internal class Program
    {
        [STAThread]
        static void Main()
        {
            try
            {
                OPMF.Settings.ConfigHelper.InitReadonlySettings();
                OPMF.Filesystem.FolderSetup.EstablishFolders();
                OPMF.Settings.ConfigHelper.EstablishConfig();

                var app = new App();
                app.InitializeComponent();
                app.Run();
            }
            catch (Exception e)
            {
                OPMF.Logging.Logger.GetCurrent().LogEntry(new OPMF.Entities.OPMFError(e));
                System.Console.WriteLine(e.Message);
            }
}
    }
}
