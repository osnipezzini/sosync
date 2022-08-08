using SOSyncView.ViewModels;

namespace SOSyncView.Pages;

public partial class SyncPage : ContentPage
{
	private readonly SyncViewModel viewModel;

	public SyncPage(SyncViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = this.viewModel = viewModel;
	}
}