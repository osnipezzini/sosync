using Microsoft.Extensions.Options;

using SOCore.Services;

using SOSync.Abstractions.Models;

namespace SOSync.Service;

public class Worker : BackgroundService
{
    private const string Message = "Aguardando {0} segundos até a próxima sincronia.";
    private readonly ILogger<Worker> _logger;
    private readonly IServiceScopeFactory serviceScopeFactory;
    private ISyncRunner syncRunner;
    private ILicenseService licenseService;
    private readonly SOSyncConfig settings;

    public Worker(ILogger<Worker> logger, IServiceScopeFactory serviceScopeFactory, IOptions<SOSyncConfig> options)
    {
        _logger = logger;
        this.serviceScopeFactory = serviceScopeFactory;
        settings = options.Value;
    }
    async Task SyncService(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogInformation("Iniciando sincronia em: {time}", DateTimeOffset.Now);
            await syncRunner.RunAsync(cancellationToken);
            _logger.LogInformation(Message, settings.SyncDelay);

            await Task.Delay(settings.SyncDelay * 1000, cancellationToken);
        }
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using (var scope = serviceScopeFactory.CreateAsyncScope())
        {
            syncRunner = scope.ServiceProvider.GetRequiredService<ISyncRunner>();
            licenseService = scope.ServiceProvider.GetRequiredService<ILicenseService>();
        }

        _logger.LogInformation("Iniciando serviços da sincronia!");

        if (settings.ActivateSyncs && licenseService.HasLicense)
            await SyncService(stoppingToken);
    }
}
