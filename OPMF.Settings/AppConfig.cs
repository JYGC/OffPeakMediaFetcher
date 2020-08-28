namespace OPMF.Settings
{
    /// <summary>
    /// Stores config steetings.
    /// </summary>
    public class AppConfig
    {
        public int NewChannelPastVideoDayLimit { get; set; } = 28;
        public string GoogleClientSecretPath { get; set; } = "gmail-python-quickstart.json";
    }
}
