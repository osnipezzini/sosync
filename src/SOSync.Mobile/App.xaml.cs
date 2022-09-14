using SOFramework.Services;

namespace SOSync.Mobile;

public partial class App : Application
{
	private readonly IUILicenseService licenseService;
	internal static IDispatcherTimer timer;
	public App(IUILicenseService licenseService)
	{
		InitializeComponent();

		timer = Dispatcher.CreateTimer();

        MainPage = new AppShell();
		this.licenseService = licenseService;
	}

	protected override void OnStart()
	{
		base.OnStart();

		licenseService.ValidateOnStart();

		timer.Start();
	}

	protected override void OnSleep()
	{
		base.OnSleep();

		licenseService.ValidateOnSleep();

		timer.Stop();
	}
}
