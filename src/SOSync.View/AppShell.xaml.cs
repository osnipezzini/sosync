using SOSync.View.Pages;

namespace SOSync.View;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute(nameof(SyncDetailPage), typeof(SyncDetailPage));
	}
}
