using SOLogging;
using CommunityToolkit.Maui;

namespace SOSync.Mobile;

public static class MauiProgram
{
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

		return builder.Build();
	}
}
