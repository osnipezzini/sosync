global using SOSync.Service;
global using SOSync.Common.Services;
using SOFramework;
using SOLogging;
using SOSync.Common;
using SOCore;

[assembly: SOApplication(AppConstants.Identifier, AppName = AppConstants.AppName,
    ModuleId = AppConstants.ServiceID,
    ModuleName = "Worker",
    LogName = "SOSync")]

#if DEBUG
Environment.SetEnvironmentVariable("SOLOGLEVEL", "10");
Environment.SetEnvironmentVariable("SOTECHDEV", "1");
Environment.SetEnvironmentVariable("SODEBUG", "1");
#endif

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services
            .AddHostedService<Worker>()
            .AddScoped<ISyncRunner, SyncRunner>()
            .AddScoped<IDbService, DbService>()
            .AddSOLicense();
    })
    .UseSOLogging()
    .Build();

await host.UseSOLicenseAsync();
