namespace HUMap.Views;

public sealed partial class TimetableDetailPage
{
    public TimetableDetailPage(TimetableDetailViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    private async void OnViewLocationClicked(object sender, EventArgs e)
    {
        var httpclient = new HttpClient();
        var geocodingService = new GeocodingService(httpclient);
        var button = (Button)sender;
        if (button.BindingContext is not TimetableItem item) return;
        var location = item.Location;
        location = "Hull University HU6 " + location;
        try
        {
            var (latitude, longitude) = await geocodingService.GetCoordinatesAsync(location);
            await Shell.Current.GoToAsync("///MapPage");
            Debug.WriteLine(latitude);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
    }
}