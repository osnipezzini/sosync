using Microsoft.AppCenter.Crashes;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Text;
using System.Text.Json;

namespace SOSync.Common.Services;

public interface ISyncAPIService
{
    Task GetStatusSync();
}
public class SyncAPIService : ISyncAPIService
{
    private readonly HttpClient? httpClient;
    private readonly ILogger<SyncAPIService>? logger;

    public SyncAPIService(IServiceProvider serviceProvider)
    {
        httpClient = serviceProvider.GetService<HttpClient>();
        if (httpClient is not null)
            httpClient.BaseAddress = new Uri("");
        logger = serviceProvider.GetService<ILogger<SyncAPIService>>();
    }

    public async Task GetStatusSync()
    {
        string message = "";
        string path = "/api/status";
        if (httpClient is null || logger is null)
            throw new Exception("HttpClient ou Logger não definido!");
        try
        {
            StringContent httpContent = new("", Encoding.Default, "application/json");
            logger.LogDebug("------------------------------------------------------------------");
            logger.LogDebug($"Enviando requisição para pegar os status da sincronia.");
            logger.LogDebug($"Path: {path}");
            logger.LogDebug("------------------------------------------------------------------");
            HttpResponseMessage response = await httpClient.GetAsync(path);
            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    break;
                case HttpStatusCode.NotFound:
                    message = $"Rota não encontrada: {path}";
                    logger.LogError(message);
                    throw new Exception(message);
                default:
                    message = $"O servidor retornou um status não mapeado: {path}";
                    logger.LogError(message);
                    throw new Exception(message);
            }
        }
        catch (HttpRequestException hre)
        {
            Crashes.TrackError(hre);
            logger.LogError($"Não conseguimos contatar a API em tempo hábil.");
        }
        catch (Exception e)
        {
            Crashes.TrackError(e);
            logger.LogDebug("------------------------------------------------------------------");
            logger.LogDebug($"Ocorreu um erro fatal ao carregar os dados na API.");
            logger.LogDebug($"Path: {path}");
            logger.LogDebug("------------------------------------------------------------------");
            throw new Exception(e.Message, e);
        }
    }
}

