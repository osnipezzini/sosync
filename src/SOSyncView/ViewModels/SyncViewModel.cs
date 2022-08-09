using SOFramework;

using System.Collections.ObjectModel;

namespace SOSyncView.ViewModels;

public class SyncViewModel : SOViewModel
{
    public ObservableCollection<Sync> Bombas { get; set; }

}
