using SOSync.Common.Services;
using SOSync.Common.Utils;

namespace SOSync.Mobile.ViewModels;

public partial class SyncViewModel : SOViewModel
{
    [ObservableProperty]
    private Sync selectedSync;
    private bool isVisible;
    private readonly ISyncAPIService aPIService;


    public bool IsVisible
    {
        get => isVisible; set
        {
            if (Syncs.Count > 0)
                isVisible = false;
            else isVisible = true;
        } }
    public ObservableCollection<Sync> Syncs { get; }
    public Command RefreshStatusCommand { get; }
    public SyncViewModel(ISyncAPIService syncAPIService)
	{
		Syncs = new ObservableCollection<Sync>();
        RefreshStatusCommand = new Command(async () => await ExecuteRefreshStatusCommand());
        aPIService = syncAPIService;
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
    private async Task RefreshSyncList()
    {
        IsBusy = true;

        var bombas = await aPIService.GetStatusSync();

        if (bombas is not null)
        {
            Syncs.Clear();
            foreach (var bomba in bombas)
                Syncs.Add(bomba);
        }

        IsBusy = false;
    }
}
