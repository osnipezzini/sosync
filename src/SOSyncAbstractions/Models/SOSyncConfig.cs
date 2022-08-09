namespace SOSyncAbstractions.Models
{
    public class SOSyncConfig
    {
        public bool ReplaceSyncFiles { get; set; } = true;
        public IEnumerable<DatabaseConfig> Databases { get; set; } = Array.Empty<DatabaseConfig>();
        public int SyncMaxTime { get; set; }
    }
}
