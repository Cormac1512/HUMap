using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class GeocodingService
{
    private readonly HttpClient _httpClient;

    public GeocodingService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<(double, double)> GetCoordinatesAsync(string location)
    {
        var response = await _httpClient.GetAsync($"https://maps.googleapis.com/maps/api/geocode/json?address={location}&key=AIzaSyDW5Rmx15kBSb0_7kKfYGka8Zsr5hX--bQ");

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        var geocode = JsonConvert.DeserializeObject<Geocode>(json);

        return (geocode.Results[0].Geometry.Location.Latitude, geocode.Results[0].Geometry.Location.Longitude);
    }

    private class Geocode
    {
        [JsonProperty("results")]
        public Result[] Results { get; set; }
    }

    private class Result
    {
        [JsonProperty("geometry")]
        public Geometry Geometry { get; set; }
    }

    private class Geometry
    {
        [JsonProperty("location")]
        public Location Location { get; set; }
    }

    private class Location
    {
        [JsonProperty("lat")]
        public double Latitude { get; set; }

        [JsonProperty("lng")]
        public double Longitude { get; set; }
    }
}