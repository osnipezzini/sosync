using SOSync.View.ViewModels;

namespace SOSync.View.Pages;

public partial class SyncPage : ContentPage
{
    private readonly SyncViewModel viewModel;

    public SyncPage(SyncViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = this.viewModel = viewModel;
    }

    void Handle_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem != null)
        {
            viewModel.ShowSyncDetailCommand.Execute(e.SelectedItem);
        }
    }
}