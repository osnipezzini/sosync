namespace SOSync.Mobile;

public partial class App : Application
{
	internal static IDispatcherTimer timer;
	public App()
	{
		InitializeComponent();

		timer = Dispatcher.CreateTimer();

        MainPage = new AppShell();
	}

	protected override void OnStart()
	{
		base.OnStart();

		timer.Start();
	}

	protected override void OnSleep()
	{
		base.OnSleep();

		timer.Stop();
	}
}
