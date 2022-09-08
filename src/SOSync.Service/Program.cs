using SOCore;
using SOCore.Exceptions;
using SOCore.Services;

using SOCore.Utils;
using SOFramework;
using SOLogging;
using SOSync.Common;
using SOSync.Service;
using System.Globalization;

CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("pt-BR");

#if DEBUG
Environment.SetEnvironmentVariable("SOLOGLEVEL", "10");
Environment.SetEnvironmentVariable("SODEBUG", "1");
//Environment.SetEnvironmentVariable("SOTECHDEV", "1");
#endif


var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddSOLicense();
        services.AddHostedService<Worker>();
        services.ConfigureCommonServices();
    })
    .UseSOLogging()
    .Build();

await host.UseSOLicenseAsync();