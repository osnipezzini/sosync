using Microsoft.AppCenter.Crashes;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Text;

namespace SOSync.Common.Services;

public interface ISyncAPIService
{
    Task<List<Sync>?> GetStatusSync();
}
public class SyncAPIService : ISyncAPIService
{
    private readonly HttpClient? httpClient;
    private readonly ILogger<SyncAPIService>? logger;

    public SyncAPIService(IServiceProvider serviceProvider)
    {
        httpClient = serviceProvider.GetService<HttpClient>();
        if (httpClient is not null)
            httpClient.BaseAddress = new Uri("https://c5ba-2804-30c-166d-fa00-5d5b-4e70-5847-57b8.sa.ngrok.io");
        logger = serviceProvider.GetService<ILogger<SyncAPIService>>();
    }

    public async Task<List<Sync>?> GetStatusSync()
    {
        string message = "";
        string path = "/api/Status/list";
        if (httpClient is null || logger is null)
            throw new Exception("HttpClient ou Logger não definido!");
        try
        {
            StringContent httpContent = new("", Encoding.Default, "application/json");
            logger.LogDebug("------------------------------------------------------------------");
            logger.LogDebug($"Enviando requisição para pegar os status da sincronia.");
            logger.LogDebug($"Path: {path}");
            logger.LogDebug("------------------------------------------------------------------");
            var response = await httpClient.GetAsync<List<Sync>?>(path);
            switch (response.StatusCode)
            {
                case HttpStatusCode.OK:
                    return response.Value;
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
            throw new Exception(hre.Message, hre);
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

