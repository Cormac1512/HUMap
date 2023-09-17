using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using Map = Microsoft.Maui.Controls.Maps.Map;

namespace HUMap.Views;

public sealed partial class MapPage
{
    private readonly Dictionary<Polygon, Location> _centroidCache = new();
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

        if (Preferences.Default.Get("firstStart", "") == "true")
        {
            await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            Preferences.Default.Set("firstStart", "false");
            await DisplayAlert("How to use", "Click on a building, the name will display at the bottom of your screen", "Ok");
        }
        if (!Preferences.Default.ContainsKey("location")) return;
        var locationStr = Preferences.Default.Get("location", "");
        if (string.IsNullOrWhiteSpace(locationStr)) return;

        try
        {
            var geocodingService = new GeocodingService();
            var (latitude, longitude) = await GeocodingService.GetCoordinatesAsync(locationStr, _map);
            var location = new Location(latitude, longitude);
            var mapSpan = MapSpan.FromCenterAndRadius(location, Distance.FromKilometers(0.07));
            _map.MoveToRegion(mapSpan);
            PolyClick(location);
        }
        catch
        {
            // handle any exceptions here
        }

        Preferences.Default.Set("location", "");
    }

    private static Location GetPolygonCentroid(Polygon polygon)
    {
        var sumLat = 0.0;
        var sumLong = 0.0;
        var count = polygon.Geopath.Count;

        foreach (var point in polygon.Geopath)
        {
            sumLat += point.Latitude;
            sumLong += point.Longitude;
        }

        var center = new Location
        {
            Latitude = sumLat / count,
            Longitude = sumLong / count
        };
        return center;
    }

    private static bool IsPointInPolygon(Location point, IList<Location> polygon)
    {
        var x = point.Latitude;
        var y = point.Longitude;
        var isInside = false;
        var lockObject = new object(); // Create an object to use for locking

        Parallel.For(0, polygon.Count, (i) =>
        {
            var j = (i == 0) ? polygon.Count - 1 : i - 1;
            var xi = polygon[i].Latitude;
            var yi = polygon[i].Longitude;
            var xj = polygon[j].Latitude;
            var yj = polygon[j].Longitude;

            var intersect = yi > y != yj > y && x < (xj - xi) * (y - yi) / (yj - yi) + xi;
            if (!intersect) return;
            lock (lockObject)
            {
                isInside = !isInside;
            }
        });

        return isInside;
    }

    private Location GetCachedPolygonCentroid(Polygon polygon)
    {
        if (_centroidCache.TryGetValue(polygon, out var cachedCentroid))
        {
            return cachedCentroid;
        }

        var newCentroid = GetPolygonCentroid(polygon);
        _centroidCache[polygon] = newCentroid;

        return newCentroid;
    }

    private void OnMapClicked(object sender, MapClickedEventArgs e)
    {
        Location clickEventCoordinates = new(e.Location.Latitude, e.Location.Longitude);
        if (PolyClick(clickEventCoordinates))
        {
            Toast.Make(_selected.ClassId, ToastDuration.Long, 17).Show();
        }
    }

    private bool PolyClick(Location location)
    {
        _map.Pins.Clear();
        var polygons = _map.MapElements.OfType<Polygon>();
        var selectedPolygon = polygons.AsParallel().FirstOrDefault(polygon => IsPointInPolygon(location, polygon.Geopath));

        if (selectedPolygon == null) return false;
        _selected = selectedPolygon;
        var polygonCentre = GetCachedPolygonCentroid(_selected);
        var pin = new Pin
        {
            Label = _selected.ClassId,
            Type = PinType.Place,
            Location = polygonCentre
        };

        lock (_map.Pins)
        {
            _map.Pins.Add(pin);
        }

        return true;
    }
}