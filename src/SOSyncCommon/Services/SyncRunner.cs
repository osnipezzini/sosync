namespace SOSyncCommon.Services;
public interface ISyncRunner
{
    DateTime TimeStarted { get; }
    Task RunAsync(CancellationToken cancellationToken);
}
public class SyncRunner : ISyncRunner
{
    private DateTime _startTime;
    private readonly ILogger<SyncRunner> _logger;
    private readonly IDbService dbService;
    private readonly SOSyncConfig _syncConfig;
    public DateTime TimeStarted { get => _startTime; }

    public SyncRunner(ILogger<SyncRunner> logger, IOptions<SOSyncConfig> options, IDbService dbService)
    {
        _logger = logger;
        this.dbService = dbService;
        _syncConfig = options.Value;
    }
    private static void Prepare()
    {
        try
        {
            Directory.SetCurrentDirectory("C:\\autosystem\\");
            if (!File.Exists("sincronia.exe"))
            {
                File.Copy("main.exe", "sincronia.exe");
            }
            else if (!PathUtil.FileCompare("main.exe", "sincronia.exe"))
            {
                File.Delete("sincronia.exe");
                File.Copy("main.exe", "sincronia.exe");
            }
        }
        catch (Exception ex)
        {

            Console.WriteLine(ex.Message);
        }

    }
    private async Task<bool> RunSyncAsync(string nickname, string proc = "sync")
    {
        var workDir = "C:\\autosystem";
        var filename = "C:\\autosystem\\sincronia.exe";
        int exitDelay;
        var args = "";

        if (OperatingSystem.IsLinux())
        {
            filename = "as_sync";
            workDir = "/usr/local/bin/";

            if (proc == "estoque")
            {
                filename = "as_main";
                args += " --estoque";
                exitDelay = 120;
            }
            else
                exitDelay = _syncConfig.SyncMaxTime;
        }
        else
        {
            if (proc == "estoque")
            {
                args += " --estoque";
                exitDelay = 120;
            }
            else
            {
                args += " --sync --nogui";
                exitDelay = _syncConfig.SyncMaxTime;
            }
        }

        try
        {
            var conf = _syncConfig.Databases
                .First(dbconfig => dbconfig.Nickname == nickname);

            if (nickname.ToLower() != "central")
            {
                args += " --db-profile=" + nickname.ToUpper();
            }

            if (!conf.HasConnection(_logger))
                return false;

            //var result = $"{filename} {args}".Bash(workDir);

            //string[] results = result.Split('\n');
            //results.ToList().ForEach(x => logger.Information(x));
            var result = await PathUtil.StartProcessAsync(filename, args, workDir, exitDelay);
            _logger.LogInformation("Sincronia concluída .");
            //Console.WriteLine(result);
            return true;

        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            Console.WriteLine(ex.Message);
            return false;
        }
    }
    public async Task RunAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Preparando arquivos .");
        if (OperatingSystem.IsWindows())
            Prepare();

        Thread.Sleep(2000);
        //SetTimer();
        foreach (var item in _syncConfig.Databases)
        {
            if (item.ActiveSync)
            {
                _logger.LogInformation($"Iniciando sincronia da configuracao {item.Nickname}.");
                _startTime = DateTime.Now;

                await item.KillSyncAsync();
                var lastUpdate = DateTime.Now - item.LastMaintenance;
                var minTime = TimeSpan.FromHours(16);
                _logger.LogInformation($"Ultima manutenção de tabelas : {item.LastMaintenance}");
                _logger.LogInformation($"Tempo minimo : {minTime}");
                if (item.LastMaintenance is null || lastUpdate > minTime)
                {
                    await dbService.ClearTables(item);
                    if (OperatingSystem.IsWindows())
                        await RunSyncAsync(item.Nickname, "estoque");
                    item.LastMaintenance = DateTime.Now;
                }
                await RunSyncAsync(item.Nickname);
            }
            Thread.Sleep(10000);
        }
    }
}
