using CommunityToolkit.Maui;
using SOLogging;
using SOSync.Common.Services;

namespace SOSync.Mobile;

public static class MauiProgram
{
    private static readonly IServiceCollection _services = new ServiceCollection();
    public static MauiApp CreateMauiApp()
    {
#if DEBUG
        Environment.SetEnvironmentVariable("SOLOGLEVEL", "10");
        Environment.SetEnvironmentVariable("SOTECHDEV", "1");
        Environment.SetEnvironmentVariable("SODEBUG", "1");
#endif

        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseSOFramework()
            .ConfigureSOLicense()
            .UseMauiCommunityToolkit()
            .ConfigureSOLogging(x =>
            {
                x.AppCenterKey = AppConstants.AppCenterKey;
                x.ApplicationInsightsKey = AppConstants.AppInsightsKey;
            });

        builder.RegisterViews(typeof(MauiProgram));

        builder.Services.Init();

        return builder.Build();
    }

    public static void Init(this IServiceCollection _services)
    {
        _services.AddScoped<ISyncAPIService, SyncAPIService>();
    }

    public static T GetService<T>()
    {
        return _services.BuildServiceProvider().GetService<T>();
    }

}
