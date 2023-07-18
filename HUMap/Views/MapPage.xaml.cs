using Microsoft.Maui.Controls.Maps;
using Map = Microsoft.Maui.Controls.Maps.Map;

namespace HUMap.Views;

public sealed partial class MapPage
{
    private Polygon _selected;

    public MapPage(MapViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    //TODO: add onappearing to change camera location if cooridnated are saved in preferences
    private void OnMapClicked(object sender, MapClickedEventArgs e)
    {
        var current = _selected;
        Location clickEventCoordinates = new(e.Location.Latitude, e.Location.Longitude);
        var map = (Map)sender;
        var poly = map.MapElements.OfType<Polygon>().ToList();

        foreach (var polygon in from polygon in poly
                                let polygonCoordinates = polygon.Geopath
                                let isWithinPolygon = IsPointInPolygon(clickEventCoordinates, polygonCoordinates)
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
            DisplayAlert(_selected.ClassId, _selected.AutomationId, "OK");
        }

        if (current != _selected || _selected == null) return;
        _selected.FillColor = Color.FromArgb("#88FF9900");
        _selected.StrokeColor = Color.FromArgb("#FF9900");
        _selected = null;
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