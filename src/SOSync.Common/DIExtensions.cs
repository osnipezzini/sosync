using Microsoft.Extensions.DependencyInjection;
using SOSync.Common.Services;

namespace SOSync.Common;

public static class DIExtensions
{
    public static void ConfigureCommonServices(this IServiceCollection services)
    {
        services.AddScoped<IDbService, DbService>();
        services.AddScoped<ISyncAPIService, SyncAPIService>();
        services.AddScoped<ISyncRunner, SyncRunner>();
    }
}
