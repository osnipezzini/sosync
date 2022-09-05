using Microsoft.Extensions.Logging;
using SOSync.Abstractions.Models;
using SOSync.Common;
using SOSync.Common.Services;
using SOSync.Domain.Interfaces;

namespace SOSync.Domain.Services;

public class APIService : IAPIService
{
    private readonly ILogger<APIService> _logger;
    private readonly IProgService progService;
    public APIService(ILogger<APIService> logger, IProgService progService)
    {
        this.progService = progService;
        _logger = logger;
    }

    public async Task<List<Sync>> GetSyncs()
    {
        try
        {
            if (AppSettings.DatabaseConfig.Count == 0)
                await progService.TryAutoRegisterDbConfigAsync();

            var syncs = new List<Sync>();
            foreach (var item in AppSettings.DatabaseConfig)
            {
                var newSync = await item.ListSyncStatus();

                if (newSync != null)
                {
                    foreach (var sync in newSync)
                    {
                        syncs.Add(sync);
                    }
                }
            }
            return syncs;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }

    }
}
