namespace SOSyncCommon.Services;

public interface IDbService
{
    Task ClearTables(DatabaseConfig databaseConfig);
}

public class DbService : IDbService
{
    private readonly ILogger<DbService> _logger;
    private readonly SOSyncConfig _config;
    public DbService(ILogger<DbService> logger, IOptions<SOSyncConfig> options)
    {
        _logger = logger;
        _config = options.Value;
    }

    private void PostgresNotification(object sender, NpgsqlNotificationEventArgs e)
    {

        _logger.LogDebug(e.Payload);
    }

    public async Task ClearTables(DatabaseConfig databaseConfig)
    {
        try
        {
            await databaseConfig.ClearTablesAsync(_config.ReplaceSyncFiles);
        }
        catch (Exception ex)
        {
            _logger.LogError("Ocorreu um erro ao tentar limpar uma tabela .");
            _logger.LogError(ex.Message);
        }
        await databaseConfig.ExecuteContaSaldoCheckAsync();
    }
}
