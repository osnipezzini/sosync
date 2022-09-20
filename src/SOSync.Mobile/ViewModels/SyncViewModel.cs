using Microsoft.Extensions.Logging;
using SOCore.Models;
using SOCore.Services;
using SOSync.Common.Services;

namespace SOSync.Mobile.ViewModels;

public partial class SyncViewModel : SOViewModel
{
    [ObservableProperty]
    private Sync selectedSync;
    private bool isVisible;
    private readonly ISyncAPIService aPIService;
    private readonly ILogger logger;
    private readonly ILicenseService license;


    public bool IsVisible
    {
        get => isVisible; set
        {
            if (Syncs.Count > 0)
                isVisible = false;
            else isVisible = true;
        }
    }
    public ObservableCollection<Sync> Syncs { get; }
    public Command RefreshStatusCommand { get; }
    public SyncViewModel(ISyncAPIService syncAPIService, ILogger<SyncViewModel> logger, IServiceProvider serviceProvider)
    {
        this.license = serviceProvider.GetRequiredService<ILicenseService>();
        this.logger = logger;
        Syncs = new ObservableCollection<Sync>();
        RefreshStatusCommand = new Command(async () => await ExecuteRefreshStatusCommand());
        aPIService = syncAPIService;
    }

    private async Task ExecuteRefreshStatusCommand()
    {
        await RefreshSyncList();
    }

    [RelayCommand]
    private Task ShowSyncDetail(Sync sync) => Shell.Current.GoToAsync(nameof(SyncDetailPage), new Dictionary<string, object>
        {
            {"Sync", sync }
        });


    public override Task OnAppearing()
    {
        App.timer.Interval = new TimeSpan(0, 0, 60);
        App.timer.Tick += Timer_Tick;

        return base.OnAppearing();
    }

    private async void Timer_Tick(object sender, EventArgs e)
    {
        await RefreshSyncList();
    }

    [RelayCommand]
    private async Task RefreshSyncList()
    {
        IsBusy = true;

        try
        {
            if (!license.IsValid)
                return;

            var syncs = await aPIService.GetStatusSync();

            if (syncs is not null)
            {
                Syncs.Clear();
                foreach (var sync in syncs)
                    Syncs.Add(sync);
            }
        }
        catch (Exception ex)
        {
            logger.LogDebug(ex.StackTrace);
            logger.LogError(ex, "Erro ao buscar lista de sincronias");
#if ANDROID
            await DisplayAlert(ex.Message, "ERRO FATAL");
#endif
            await Shell.Current.DisplayAlert(ex.Message, "ERRO FATAL", "Ok");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
