using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using Map = Microsoft.Maui.Controls.Maps.Map;

namespace HUMap.Views;

public sealed partial class MapPage
{
    private readonly Map _map;
    private Polygon _selected;

    public MapPage(MapViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        _map = FindByName("map1") as Map;
    }

    protected override async void OnAppearing()
    {
        if (!Preferences.Default.ContainsKey("location")) return;
        if (Preferences.Default.Get("location", "") == "") return;
        try
        {
            var locationStr = Preferences.Default.Get("location", "");
            var httpclient = new HttpClient();
            var geocodingService = new GeocodingService(httpclient);
            var (latitude, longitude) = await geocodingService.GetCoordinatesAsync(locationStr, _map);
            var location = new Location(latitude, longitude);
            var mapSpan = MapSpan.FromCenterAndRadius(location, Distance.FromKilometers(0.07));
            _map.MoveToRegion(mapSpan);
            PolyClick(location);
        }
        catch
        {
            //d
        }

        Preferences.Default.Set("location", "");
    }

    private bool PolyClick(Location location)
    {
        var current = _selected;
        var poly = _map.MapElements.OfType<Polygon>().ToList();

        foreach (var polygon in from polygon in poly
                                let polygonCoordinates = polygon.Geopath
                                let isWithinPolygon = IsPointInPolygon(location, polygonCoordinates)
                                where isWithinPolygon
                                where polygon != _selected
                                select polygon)
        {
            polygon.FillColor = Color.FromArgb("#881BA1E2"); //change to colours file
            polygon.StrokeColor = Color.FromArgb("#681BA1E2");
            if (_selected != null)
            {
                _selected.FillColor = Color.FromArgb("#88FF9900");
                _selected.StrokeColor = Color.FromArgb("#FF9900");
            }

            _selected = polygon;
            return true;
        }

        if (current != _selected || _selected == null) return false;
        _selected.FillColor = Color.FromArgb("#88FF9900");
        _selected.StrokeColor = Color.FromArgb("#FF9900");
        _selected = null;
        return false;
    }

    private void OnMapClicked(object sender, MapClickedEventArgs e)
    {
        Location clickEventCoordinates = new(e.Location.Latitude, e.Location.Longitude);
        if (PolyClick(clickEventCoordinates)) DisplayAlert(_selected.ClassId, _selected.AutomationId, "Ok");
    }

    private static bool IsPointInPolygon(Location point, IList<Location> polygon)
    {
        var x = point.Latitude;
        var y = point.Longitude;
        var isInside = false;

        for (int i = 0, j = polygon.Count - 1; i < polygon.Count; j = i++)
        {
            var xi = polygon[i].Latitude;
            var yi = polygon[i].Longitude;
            var xj = polygon[j].Latitude;
            var yj = polygon[j].Longitude;

            var intersect = yi > y != yj > y && x < (xj - xi) * (y - yi) / (yj - yi) + xi;
            if (intersect) isInside = !isInside;
        }

        return isInside;
    }
}