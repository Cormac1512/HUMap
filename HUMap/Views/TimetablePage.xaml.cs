namespace HUMap.Views;

public sealed partial class TimetablePage
{
    private readonly TimetableViewModel _viewModel;

    public TimetablePage(TimetableViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        if (Preferences.ContainsKey("firstStartMain"))
        {
            if (Preferences.Get("firstStartMain", "") == "true")
            {
                await DisplayAlert("Instructions", "Click a timetable item to view more information about the event", "OK");
                Preferences.Set("firstStartMain", "false");
            }
        }

        var lastLoadTime = DateTime.MinValue;
        if (Preferences.ContainsKey("LastLoadTime")) lastLoadTime = Preferences.Get("LastLoadTime", DateTime.MinValue);
        if (!((DateTime.Now - lastLoadTime).TotalHours >= 0.25)) return;
        await _viewModel.OnRefreshing();
        Preferences.Set("LastLoadTime", DateTime.Now);
    }
}