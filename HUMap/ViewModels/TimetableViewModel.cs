﻿namespace HUMap.ViewModels;

public partial class TimetableViewModel : BaseViewModel
{
    private readonly TimetableService dataService;

    [ObservableProperty] private bool isRefreshing;

    [ObservableProperty] private ObservableCollection<TimetableItem> items;

    public TimetableViewModel(TimetableService service)
    {
        dataService = service;
    }

    public async Task LoadDataAsync()
    {
        Items = new ObservableCollection<TimetableItem>(await TimetableService.GetItems());
    }

    [RelayCommand]
    public async Task OnRefreshing()
    {
        IsRefreshing = true;

        try
        {
            await Task.Run(LoadDataAsync);
        }
        finally
        {
            IsRefreshing = false;
        }
    }

    [RelayCommand]
    private async void GoToDetails(TimetableItem item)
    {
        if (item.Title == "Setup")
        {
            await Shell.Current.GoToAsync("///SettingsRoute");
        }
        if (!item.IsNotDayOfWeekItem) return;

        await Shell.Current.GoToAsync(nameof(TimetableDetailPage), true, new Dictionary<string, object>
        {
            { "Item", item }
        });
    }
}