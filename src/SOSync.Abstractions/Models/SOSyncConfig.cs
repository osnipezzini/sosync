namespace SOSync.Abstractions.Models
{
    public class SOSyncConfig
    {
        public bool ReplaceSyncFiles { get; set; } = true;
        public IEnumerable<DatabaseConfig> Databases { get; set; } = Array.Empty<DatabaseConfig>();
        public int SyncMaxTime { get; set; } = 120;
        public int SyncDelay { get; set; } = 60;
        public bool ActivateSyncs { get; set; }
    }
}
