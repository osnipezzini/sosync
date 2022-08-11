namespace SOSync.Mobile.Pages;

public partial class SyncPage : ContentPage
{
    private readonly SyncViewModel viewModel;

    public SyncPage(SyncViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = this.viewModel = viewModel;
    }

    protected override void OnAppearing()
    {
        viewModel?.RefreshSyncListCommand?.Execute(null);
    }

    void Handle_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem != null)
        {
            viewModel.ShowSyncDetailCommand.Execute(e.SelectedItem);
        }
    }
}