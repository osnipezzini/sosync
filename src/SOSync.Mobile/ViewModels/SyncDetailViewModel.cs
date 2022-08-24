using SOSync.Common.Utils;

namespace SOSync.Mobile.ViewModels
{
    public partial class SyncDetailViewModel : SOViewModel, IQueryAttributable
    {
        [ObservableProperty]
        private Sync sync;
        private Color color;

        public Color Color { get => color; set => SetProperty(ref color, value); }
        public SyncDetailViewModel()
        {
            Title = $"Detalhes sincronia";
        }

        private void GetColor()
        {
            if (sync is null){
                Color = Color.FromHex("#000000");
                return;
            }
            else if (sync.Status == StatusImages.OK)
                Color = Color.FromHex("#4bec00");
            else
                Color = Color.FromHex("#FF5525");
        }

        [RelayCommand]
        public Task GoBack() => Shell.Current.GoToAsync("..");
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.ContainsKey(nameof(Sync)) && query[nameof(Sync)] is Sync sync)
                Sync = sync;

            GetColor();
        }
    }
}
