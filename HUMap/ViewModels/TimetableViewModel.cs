namespace HUMap.ViewModels;

public partial class TimetableViewModel : BaseViewModel
{
    private readonly TimetableService dataService;

    [ObservableProperty] private bool isRefreshing;

    [ObservableProperty] private ObservableCollection<TimetableItem> items;

    public TimetableViewModel(TimetableService service)
    {
        dataService = service;
    }

    [RelayCommand]
    public async Task OnRefreshing()
    {
        IsRefreshing = true;

        try
        {
            await LoadDataAsync();
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    public async Task LoadDataAsync()
    {
        Items = new ObservableCollection<TimetableItem>(await TimetableService.GetItems());
    }

    [RelayCommand]
    private async void GoToDetails(TimetableItem item)
    {
        if (!item.IsNotDayOfWeekItem) return;

        await Shell.Current.GoToAsync(nameof(TimetableDetailPage), true, new Dictionary<string, object>
        {
            { "Item", item }
        });
    }
}