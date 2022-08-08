using SOFramework;

using SOSyncView.Pages;
using SOSyncView.ViewModels;

namespace SOSyncView;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseSOFramework()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});

		builder.Services.RegisterPages();

		return builder.Build();
	}

	private static void RegisterPages(this IServiceCollection services)
	{
		services.AddTransient<SyncViewModel>();
		services.AddTransient<SyncPage>();
	}
}
