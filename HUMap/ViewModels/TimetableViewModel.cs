namespace HUMap.ViewModels;

public partial class TimetableViewModel : BaseViewModel
{
    private readonly TimetableService dataService;

    [ObservableProperty]
    private bool isRefreshing;

    [ObservableProperty]
    private ObservableCollection<TimetableItem> items;

    public TimetableViewModel(TimetableService service)
    {
        dataService = service;
    }

    [RelayCommand]
    private async void OnRefreshing()
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

    [RelayCommand]
    public async Task LoadMore()
    {
        var items = await dataService.GetItems();

        foreach (var item in items)
        {
            Items.Add(item);
        }
    }

    public async Task LoadDataAsync()
    {
        Items = new ObservableCollection<TimetableItem>(await dataService.GetItems());
    }

    [RelayCommand]
    private async void GoToDetails(TimetableItem item)
    {
        await Shell.Current.GoToAsync(nameof(TimetableDetailPage), true, new Dictionary<string, object>
        {
            { "Item", item }
        });
    }
}