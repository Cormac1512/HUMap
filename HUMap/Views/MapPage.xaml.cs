using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps.Handlers;
using Map = Microsoft.Maui.Controls.Maps.Map;

namespace HUMap.Views;

public partial class MapPage : ContentPage
{
    private Polygon selected;

    public MapPage(MapViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
#if WINDOWS
        // Note that the map control is not supported on Windows.
        // For more details, see https://learn.microsoft.com/en-us/dotnet/maui/user-interface/controls/map?view=net-maui-7.0
        // For a possible workaround, see https://github.com/CommunityToolkit/Maui/issues/605
        Content = new Label() { Text = "Windows does not have a map control. 😢" };
#endif
    }

    private void OnMapClicked(object sender, MapClickedEventArgs e)
    {
        Location clickEventCoordinates = new(e.Location.Latitude, e.Location.Longitude);
        var polys = new List<Polygon>();
        Map map = (Map)sender;
        foreach (var poly in map.MapElements.OfType<Polygon>())
        {
            polys.Add(poly);
        }

        foreach (var polygon in polys)
        {
            var polygonCoordinates = polygon.Geopath;
            var isWithinPolygon = IsPointInPolygon(clickEventCoordinates, polygonCoordinates);
            if (!isWithinPolygon) continue;
            if (polygon == selected) continue;
            polygon.FillColor = Color.FromArgb("#881BA1E2");
            polygon.StrokeColor = Color.FromArgb("#681BA1E2");
            if (selected != null)
            {
                selected.FillColor = Color.FromArgb("#88FF9900");
                selected.StrokeColor = Color.FromArgb("#FF9900");
            }
            selected = polygon;
            DisplayAlert(selected.AutomationId, selected.ClassId, "OK");
        }
    }

    private bool IsPointInPolygon(Location point, IList<Location> polygon)
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

            var intersect = ((yi > y) != (yj > y)) && (x < ((xj - xi) * (y - yi)) / (yj - yi) + xi);
            if (intersect)
            {
                isInside = !isInside;
            }
        }

        return isInside;
    }
}