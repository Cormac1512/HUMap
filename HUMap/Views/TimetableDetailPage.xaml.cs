namespace HUMap.Views;

public partial class TimetableDetailPage : ContentPage
{
    private readonly GeocodingService _geocodingService;

    public TimetableDetailPage(TimetableDetailViewModel viewModel, GeocodingService geocodingService)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _geocodingService = geocodingService;
    }

    private async void OnViewLocationClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        if (button.BindingContext is not TimetableItem item) return;
        var location = item.Location;
        try
        {
            var (latitude, longitude) = await _geocodingService.GetCoordinatesAsync(location);
        }
        catch
        {
            // ignored
        }
    }
}