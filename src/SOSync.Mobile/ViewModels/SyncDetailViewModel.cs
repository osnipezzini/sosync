namespace SOSync.Mobile.ViewModels
{
    public partial class SyncDetailViewModel : SOViewModel, IQueryAttributable
    {
        [ObservableProperty]
        private Sync sync;

        [RelayCommand]
        public Task GoBack() => Shell.Current.GoToAsync("..");
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.ContainsKey(nameof(Sync)) && query[nameof(Sync)] is Sync sync)
                Sync = sync;
        }
    }
}
