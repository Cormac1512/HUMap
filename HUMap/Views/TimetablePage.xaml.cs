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
        base.OnAppearing();
        var lastLoadTime = DateTime.MinValue;
        if (Preferences.ContainsKey("LastLoadTime")) lastLoadTime = Preferences.Get("LastLoadTime", DateTime.MinValue);
        if (!((DateTime.Now - lastLoadTime).TotalHours >= 0.5)) return;
        await _viewModel.LoadDataAsync();
        Preferences.Set("LastLoadTime", DateTime.Now);
    }
}