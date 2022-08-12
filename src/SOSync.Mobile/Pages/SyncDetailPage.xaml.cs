namespace SOSync.Mobile.Pages;

public partial class SyncDetailPage : ContentPage
{
    private readonly SyncDetailViewModel viewModel;

    public SyncDetailPage(SyncDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = this.viewModel = viewModel;
    }
    protected override bool OnBackButtonPressed()
    {
        Shell.Current.GoToAsync("..");
        return true;
    }
    void OnSwiped(object sender, SwipedEventArgs e)
    {
        switch (e.Direction)
        {
            case SwipeDirection.Left:
                // Handle the swipe
                break;
            case SwipeDirection.Right:
                // Handle the swipe
                break;
            case SwipeDirection.Up:
                // Handle the swipe
                break;
            case SwipeDirection.Down:
                OnBackButtonPressed();
                break;
        }
    }
}