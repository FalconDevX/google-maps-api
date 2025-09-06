using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;



namespace WebAPI.Services
{
    public class GoogleMapsService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public GoogleMapsService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["GoogleMaps:ApiKey"] ?? throw new ArgumentNullException("Google Maps API key is not configured.");
        }

        public async Task<string> GetPlacePropertiesByNameAsync(string query)
        {
            var requestBody = JsonSerializer.Serialize(new { textQuery = query });

            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                "https://places.googleapis.com/v1/places:searchText"
            );

            request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            request.Headers.Add("X-Goog-Api-Key", _apiKey);
            request.Headers.Add("X-Goog-FieldMask", "places.displayName,places.formattedAddress,places.location,places.rating,places.userRatingCount,places.types");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetPlaceCoordinatesByNameAsync(string query)
        {
            var requestBpdy = JsonSerializer.Serialize(new { textQuery = query });

            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                "https://places.googleapis.com/v1/places:searchText"
            );

            request.Content = new StringContent(requestBpdy, Encoding.UTF8, "application/json");
            request.Headers.Add("X-Goog-Api-Key", _apiKey);
            request.Headers.Add("X-Goog-FieldMask", "places.location");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();

        }

        public async Task<string> GetNearbyPlacesByCoordinatesAsync(string query, string radius)
        {
            var json = await GetPlaceCoordinatesByNameAsync(query);
            using var doc = JsonDocument.Parse(json);

            var root = doc.RootElement;
            if (!root.TryGetProperty("places", out var places) || places.GetArrayLength() == 0)
            {
                throw new Exception("Place not found.");
            }

            var location = places[0].GetProperty("location");
            var latitude = location.GetProperty("latitude").GetDouble();
            var longitude = location.GetProperty("longitude").GetDouble();

            var requestBody = JsonSerializer.Serialize(new
            {
                locationRestriction = new
                {
                    circle = new
                    {
                        center = new
                        {
                            latitude = latitude,
                            longitude = longitude
                        },
                        radius = radius
                    }
                }
            });


            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                "https://places.googleapis.com/v1/places:searchNearby"
            );


            request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            request.Headers.Add("X-Goog-Api-Key", _apiKey);
            request.Headers.Add("X-Goog-FieldMask", "places.displayName");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
    }
}
