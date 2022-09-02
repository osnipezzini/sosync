using System.Collections.ObjectModel;

namespace SOSync.Abstractions.Models
{
    public class SOSyncConfig
    {
        public bool ReplaceSyncFiles { get; set; } = true;
        public ObservableCollection<DatabaseConfig> Databases { get; set; } = new ObservableCollection<DatabaseConfig>();
        public int SyncMaxTime { get; set; } = 120;
        public bool ActivateSyncs { get; set; } = true;
        public int SyncDelay { get; set; } = 15;
    }
}
