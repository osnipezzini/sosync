namespace SOSyncAbstractions.Models
{
    public class DatabaseConfig
    {
        private DateTime? _lastMaintenance;
        public int? Id { get; set; }
        public string Host { get; set; } = string.Empty;
        public string Port { get; set; } = "5432";
        public string Nickname { get; set; } = string.Empty;
        public string User { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public DateTime? LastMaintenance { get => _lastMaintenance ??= DateTime.Now.Subtract(TimeSpan.FromHours(48)); set => _lastMaintenance = value; }
        public bool ActiveSync { get; set; }        
    }
}
