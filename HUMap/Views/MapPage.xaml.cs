using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using Map = Microsoft.Maui.Controls.Maps.Map;

namespace HUMap.Views;

public sealed partial class MapPage
{
    private const string PolygonFillColor = "#881BA1E2";
    private const string PolygonStrokeColor = "#681BA1E2";
    private const string SelectedColor = "#88FF9900";
    private const string SelectedStrokeColor = "#FF9900";
    private static readonly HttpClient HttpClient = new();
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
        base.OnAppearing();

        if (!Preferences.Default.ContainsKey("location")) return;
        var locationStr = Preferences.Default.Get("location", "");
        if (string.IsNullOrWhiteSpace(locationStr)) return;

        try
        {
            var geocodingService = new GeocodingService(HttpClient);
            var (latitude, longitude) = await geocodingService.GetCoordinatesAsync(locationStr, _map);

            var location = new Location(latitude, longitude);
            var mapSpan = MapSpan.FromCenterAndRadius(location, Distance.FromKilometers(0.07));
            _map.MoveToRegion(mapSpan);
            PolyClick(location, true);
        }
        catch
        {
            // handle any exceptions here
        }

        Preferences.Default.Set("location", "");
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

    private void OnMapClicked(object sender, MapClickedEventArgs e)
    {
        Location clickEventCoordinates = new(e.Location.Latitude, e.Location.Longitude);
        if (PolyClick(clickEventCoordinates)) DisplayAlert(_selected.ClassId, _selected.AutomationId, "Ok");
    }

    private bool PolyClick(Location location, bool mapSSelect = false)
    {
        var current = _selected;
        var polygons = _map.MapElements.OfType<Polygon>();

        foreach (var polygon in polygons)
        {
            var polygonCoordinates = polygon.Geopath;
            var isWithinPolygon = IsPointInPolygon(location, polygonCoordinates);
            if (!isWithinPolygon || polygon == _selected)
                continue;

            polygon.FillColor = Color.FromArgb(PolygonFillColor);
            polygon.StrokeColor = Color.FromArgb(PolygonStrokeColor);
            if (_selected != null)
            {
                _selected.FillColor = Color.FromArgb(SelectedColor);
                _selected.StrokeColor = Color.FromArgb(SelectedStrokeColor);
            }

            _selected = polygon;
            return true;
        }

        if (current != _selected || _selected == null || mapSSelect) return false;
        _selected.FillColor = Color.FromArgb(SelectedColor);
        _selected.StrokeColor = Color.FromArgb(SelectedStrokeColor);
        _selected = null;
        return false;
    }
}