using SOFramework.Services;

namespace SOSync.Mobile;

public partial class App : Application
{
	private readonly IMauiLicenseService licenseService;

	public App(IMauiLicenseService licenseService)
	{
		InitializeComponent();

		MainPage = new AppShell();
		this.licenseService = licenseService;
	}

	protected override void OnStart()
	{
		base.OnStart();

		licenseService.ValidateOnStart();
	}

	protected override void OnSleep()
	{
		base.OnSleep();

		licenseService.ValidateOnSleep();
	}
}
