namespace SOSync.View.Pages;

public partial class SyncDetailPage : ContentPage
{
    public Sync Sync { get; set; }
    public SyncDetailPage()
	{
		InitializeComponent();
        BindingContext = this;
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
                base.OnBackButtonPressed();
                break;
        }
    }
}