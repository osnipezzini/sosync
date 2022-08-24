using SOSync.Common.Utils;

namespace SOSync.Mobile.ViewModels;

public partial class SyncViewModel : SOViewModel
{
    public ObservableCollection<Sync> Syncs { get; }
    [ObservableProperty]
    private Sync selectedSync;

    public Command RefreshStatusCommand { get; }
    public SyncViewModel()
	{
		Syncs = new ObservableCollection<Sync>();
        RefreshStatusCommand = new Command(async () => await ExecuteRefreshStatusCommand());
	}

    private async Task ExecuteRefreshStatusCommand()
    {
        var listSync = Syncs.ToList();
        await Task.Delay(200);

        if (listSync[1].Status == StatusImages.OK)
            listSync[1].Status = StatusImages.Delayed;
        else
            listSync[1].Status = StatusImages.OK;

        Syncs.Clear();
        foreach (var syncDict in listSync)
            Syncs.Add(syncDict);
    }

    [RelayCommand]
    private Task ShowSyncDetail(Sync sync) => Shell.Current.GoToAsync(nameof(SyncDetailPage), new Dictionary<string, object>
        {
            {"Sync", sync }
        });

    [RelayCommand]
    private void RefreshSyncList()
    {
        IsBusy = true;

        var bombas = new List<Sync>();

        Syncs.Clear();
        foreach (var bomba in bombas)
            Syncs.Add(bomba);

        IsBusy = false;
    }
}
