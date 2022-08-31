﻿using Microsoft.Extensions.DependencyInjection;
using SOSync.Common.Services;

namespace SOSync.Common;

public static class DIExtensions
{
    public static void ConfigureCommonServices(this IServiceCollection services)
    {
        services.AddSingleton<IDbService, DbService>();
        services.AddScoped<ISyncAPIService, SyncAPIService>();
        services.AddSingleton<ISyncRunner, SyncRunner>();
    }
}
