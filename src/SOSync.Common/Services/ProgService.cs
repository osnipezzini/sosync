using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using SOSync.Common.Exceptions;
using System.Net;
using System.Text;

namespace SOSync.Common.Services;

public class ProgService : IProgService
{
    private readonly ILogger<ProgService> _logger;
    private readonly HttpClient? httpClient;
    private readonly IMapper mapper;


    public ProgService(ILogger<ProgService> logger, IServiceProvider serviceProvider, IMapper mapper)
    {
        this.mapper = mapper;
        httpClient = new HttpClient();
        if (httpClient is not null)
            httpClient.BaseAddress = new Uri("http://127.0.0.1:8007");

        _logger = logger;
    }

    public async Task<bool> RunProgAsync()
    {
        var workDir = "C:\\autosystem";
        var filename = "C:\\autosystem\\manutencao.exe";
        int exitDelay = 1;
        var args = "--prog 8003";

        if (OperatingSystem.IsLinux())
        {
            filename = "as_manutencao";
            workDir = "/usr/local/bin/";
        }

        try
        {
            _logger.LogInformation("Aguardando inicialização do prog!");
            var result = await PathUtil.StartProcessAsync(filename, args, workDir, exitDelay, buffered: false);
            _logger.LogInformation("Prog iniciado com sucesso.");
            return true;

        }
        catch (Exception ex)
        {
            _logger.LogError(ex.Message);
            Console.WriteLine(ex.Message);
            return false;
        }
    }

    public async Task<DatabaseConfig> GetDbInfo()
    {
        string message = "";
        string path = "/password";
        if (httpClient is null || _logger is null)
            throw new Exception("HttpClient ou Logger não inicializados!");

        try
        {
            StringContent httpContent = new("", Encoding.Default, "application/json");
            _logger.LogDebug("------------------------------------------------------------------");
            _logger.LogDebug($"Enviando requisição para pegar as configurações padrão do banco de dados.");
            _logger.LogDebug($"Path: {path}");
            _logger.LogDebug("------------------------------------------------------------------");
            var response = await httpClient.GetAsync<DbInfo>(path);
            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    return mapper.Map<DatabaseConfig>(response.Value);
                case HttpStatusCode.NotFound:
                    message = $"Rota não encontrada: {path}";
                    _logger.LogError(message);
                    throw new Exception(message);
                case HttpStatusCode.InternalServerError:
                    throw new NotInicializedException();
                default:
                    message = $"O servidor retornou um status não mapeado: {path}";
                    _logger.LogError(message);
                    throw new Exception(message);

            }
        }
        catch (NotInicializedException nie)
        {
            _logger.LogError($"Não conseguimos contatar o prog em tempo hábil. ");
            throw new NotInicializedException(nie.Message, nie);
        }
        catch (HttpRequestException hre)
        {
            _logger.LogError($"Não conseguimos contatar o prog em tempo hábil. ");
            throw new Exception(hre.Message, hre);
        }
        catch (Exception ex)
        {
            _logger.LogDebug("------------------------------------------------------------------");
            _logger.LogDebug($"Ocorreu um erro fatal ao carregar os dados do prog. ");
            _logger.LogDebug($"Path: {path}");
            _logger.LogDebug("------------------------------------------------------------------");
            throw new Exception(ex.Message, ex);
        }
    }


    public async Task<bool> TryAutoRegisterDbConfigAsync()
    {
        var triedRegister = 0;
    retryGetDbInfo:
        try
        {
            await RunProgAsync();
            triedRegister++;

            if (triedRegister > 3)
                throw new Exception(" Numero máximo de tentativas para auto-configuração. Tente mais tarde ou realize a configuração manualmente atrávez do menu! (https://{SEUIP}:7067/)");

            await Task.Delay(9000);
            var dbInfo = await GetDbInfo();

            if (dbInfo is null)
                goto retryGetDbInfo;

            if (AppSettings.DatabaseConfig.Count > 0)
                return false;

            AppSettings.DatabaseConfig.Add(dbInfo);
            AppSettings.Save();

        }
        catch (NotInicializedException nie)
        {
            goto retryGetDbInfo;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }

        return true;
    }
}
