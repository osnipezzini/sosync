using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SOSync.Abstractions.Models;
using SOSync.Common;
using SOSync.Domain.Interfaces;

namespace SOSync.Domain.Services;

public class APIService : IAPIService
{
    private readonly ILogger<APIService> _logger;
    public APIService(ILogger<APIService> logger)
	{
		_logger = logger;
	}

	public async Task<List<Sync>> GetSyncs()
	{
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
}
