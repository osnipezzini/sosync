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

        var bombas = new Sync[]
        {
            new Sync
            {
                Atraso = 2132,
                Conexao = "Filial 1",
                Empresa = "42.211.761/0001-32",
                LastUpdate = new DateTime(2022, 8, 9, 22, 12, 21),
                Status = StatusImages.Warning
            },
            new Sync
            {
                Atraso = 2132,
                Conexao = "Filial 2",
                Empresa = "91.346.705/0001-10",
                LastUpdate = new DateTime(2022, 8, 9, 22, 12, 21),
                Status = StatusImages.Warning
            },
            new Sync
            {
                Atraso = 2132,
                Conexao = "Filial 3",
                Empresa = "72.747.047/0001-24",
                LastUpdate = new DateTime(2022, 8, 9, 22, 12, 21),
                Status = StatusImages.Warning
            },
            new Sync
            {
                Atraso = 2121212,
                Conexao = "Filial 4",
                Empresa = "12.220.762/0001-82",
                LastUpdate = new DateTime(2022, 8, 9, 20, 12, 21),
                Status = StatusImages.Delayed
            },
            new Sync
            {
                Atraso = 2132,
                Conexao = "Filial 5",
                Empresa = "92.499.948/0001-51",
                LastUpdate = new DateTime(2022, 8, 9, 22, 55, 21),
                Status = StatusImages.Warning
            },
            new Sync
            {
                Atraso = 2132,
                Conexao = "Filial 6",
                Empresa = "92.686.209/0001-79",
                LastUpdate = new DateTime(2022, 8, 9, 22, 35, 21),
                Status = StatusImages.Delayed
            },
            new Sync
            {
                Atraso = 2132,
                Conexao = "Filial 7",
                Empresa = "03.109.541/0001-40",
                LastUpdate = new DateTime(2022, 8, 9, 22, 1, 21),
                Status = StatusImages.Delayed
            },
            new Sync
            {
                Atraso = 2132,
                Conexao = "Filial 8",
                Empresa = "30.467.808/0001-44",
                LastUpdate = new DateTime(2022, 8, 9, 21, 11, 21),
                Status = StatusImages.Error
            },
            new Sync
            {
                Atraso = 2132,
                Conexao = "Filial 9",
                Empresa = "77.260.448/0001-79",
                LastUpdate = new DateTime(2022, 8, 9, 22, 7, 21),
                Status = StatusImages.NoConected
            },
        };

        Syncs.Clear();
        foreach (var bomba in bombas)
            Syncs.Add(bomba);

        IsBusy = false;
    }
}
