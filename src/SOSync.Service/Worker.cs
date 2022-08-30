using SOCore.Services;
using SOSync.Common;
using SOSync.Common.Services;

namespace SOSync.Service;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly ISyncRunner _syncRunner;
    private readonly ILicenseService _licenseService;

    public Worker(ILogger<Worker> logger, ISyncRunner syncRunner, ILicenseService licenseService)
    {
        _licenseService = licenseService;
        _syncRunner = syncRunner;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (AppSettings.SOSyncConfig.ActivateSyncs && _licenseService.HasLicense)
            await SyncService(stoppingToken);
    }

    private async Task SyncService(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation("Iniciando sincronia em: {time}", DateTimeOffset.Now);
            await _syncRunner.RunAsync(cancellationToken);
            _logger.LogInformation($"Aguardando {AppSettings.SOSyncConfig.SyncDelay} segundos até a próxima sincronia.");

            await Task.Delay(AppSettings.SOSyncConfig.SyncDelay * 1000, cancellationToken);
        }
    }
}
