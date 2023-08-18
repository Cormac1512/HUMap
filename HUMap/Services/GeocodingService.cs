using Microsoft.Maui.Controls.Maps;
using Newtonsoft.Json;
using Map = Microsoft.Maui.Controls.Maps.Map;

namespace HUMap.Services;

/// <summary>
///
/// </summary>
public sealed class GeocodingService
{
    private const double UniversityLatitude = 53.7712;
    private const double UniversityLongitude = -0.3686;
    private const double OneMileInKilometers = 1.60934;
    private readonly HttpClient _httpClient;

    /// <summary>
    ///
    /// </summary>
    /// <param name="httpClient"></param>
    public GeocodingService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="polygon"></param>
    /// <returns></returns>
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

    /// <summary>
    ///
    /// </summary>
    /// <param name="lat1"></param>
    /// <param name="lon1"></param>
    /// <param name="lat2"></param>
    /// <param name="lon2"></param>
    /// <returns></returns>
    private static double GetDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double rEarth = 6371; // Radius of the earth in km
        var latDiff = ToRadians(lat2 - lat1);
        var lonDiff = ToRadians(lon2 - lon1);
        var a = Math.Sin(latDiff / 2) * Math.Sin(latDiff / 2) +
                Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                Math.Sin(lonDiff / 2) * Math.Sin(lonDiff / 2);
        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return rEarth * c; // Distance in km
    }

    private static double ToRadians(double angle)
    {
        return Math.PI * angle / 180.0;
    }

    /// <summary>
    /// </summary>
    /// <param name="locationStr"></param>
    /// <param name="map"></param>
    /// <returns></returns>
    public async Task<(double, double)> GetCoordinatesAsync(string locationStr, Map map = null)
    {
        var location = locationStr.Trim();
        var splitLocation = location.Split('-');
        if (splitLocation.Length > 0) location = splitLocation[0].Trim();

        //search through map polygons for a matching location
        if (map != null)
        {
            var polygons = map.MapElements.OfType<Polygon>();

            // If polygons has relevant property to match location
            var location1 = location;
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

            var distance = GetDistance(geocode.Results[0].Geometry.Location.Latitude,
                geocode.Results[0].Geometry.Location.Longitude, UniversityLatitude, UniversityLongitude);

            return distance > OneMileInKilometers
                ? (UniversityLatitude,
                    UniversityLongitude) // If it's more than 1 mile away, return a default coordinate
                : (geocode.Results[0].Geometry.Location.Latitude, geocode.Results[0].Geometry.Location.Longitude);
        }
        catch
        {
            return (UniversityLatitude, UniversityLongitude);
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