using Microsoft.Maui.Controls.Maps;
using Newtonsoft.Json;
using Map = Microsoft.Maui.Controls.Maps.Map;

namespace HUMap.Services;

public sealed class GeocodingService
{
    private readonly HttpClient _httpClient;

    public GeocodingService(HttpClient httpClient)
    {
        _httpClient = httpClient;
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

        var center = new Location();
        center.Latitude = sumLat / count;
        center.Longitude = sumLong / count;
        return center;
    }

    public async Task<(double, double)> GetCoordinatesAsync(string locationStr, Map map = null)
    {
        var location = locationStr;
        try
        {
            location = location.Split("-")[0];
        }
        catch
        {
            //ignored
        }

        location = location.Trim();
        //search through map polygons for a matching location
        if (map != null)
        {
            var polygons = map.MapElements.OfType<Polygon>().ToList();

            // If polygons has relevant property to match location
            var location1 = location;
            foreach (var polygon in polygons) Debug.WriteLine(polygon.ClassId);
            foreach (var center in from polygon in polygons
                                   where polygon.ClassId == location1
                                   select GetPolygonCentroid(polygon))
                return (center.Latitude, center.Longitude);
        }

        //If it fails to find a matching polygon
        location += ", University of Hull, Hull, United Kingdom";
        try
        {
            var response = await _httpClient.GetAsync(
                $"https://maps.googleapis.com/maps/api/geocode/json?address={location}&key=AIzaSyDW5Rmx15kBSb0_7kKfYGka8Zsr5hX--bQ");

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var geocode = JsonConvert.DeserializeObject<Geocode>(json);

            return (geocode.Results[0].Geometry.Location.Latitude, geocode.Results[0].Geometry.Location.Longitude);
        }
        catch
        {
            return (53.77070899253233, -0.36903242714560686);
        }
    }

    private sealed class Geocode
    {
        [JsonProperty("results")] public Result[] Results { get; set; }
    }

    private sealed class Result
    {
        [JsonProperty("geometry")] public Geometry Geometry { get; set; }
    }

    private sealed class Geometry
    {
        [JsonProperty("location")] public Location Location { get; set; }
    }

    public sealed class Location
    {
        [JsonProperty("lat")] public double Latitude { get; set; }

        [JsonProperty("lng")] public double Longitude { get; set; }
    }
}