namespace LeadStatusUpdater.Settings
{
    public class AppSettings
    {
        public string ConnectionString { get; set; }
        public string AdminEmail { get; set; }
        public string AdminPassword { get; set; }
        public int MillisecondsDelay { get; set; }
        public int MillisecondsWhenLaunch { get; set; }
    }
}
