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

        public async Task<string> GetNearbyPlacesByCoordinatesAsync(string query, int radius, string rankPreference)
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
                            latitude,
                            longitude
                        },
                        radius = radius   
                    }
                },
                rankPreference
            });

            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                "https://places.googleapis.com/v1/places:searchNearby"
            );

            request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            request.Headers.Add("X-Goog-Api-Key", _apiKey);
            request.Headers.Add("X-Goog-FieldMask", "places.displayName,places.id,places.types,places.formattedAddress,places.rating,places.userRatingCount,places.businessStatus");


            var response = await _httpClient.SendAsync(request);
            var body = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Google API error: {response.StatusCode}, body: {body}");
            }

            return body;
        }

        public async Task<byte[]> GetPlacePhotoByNameAsync(string query, int maxWidth = 400)
        {
            var requestBody = JsonSerializer.Serialize(new { textQuery = query });

            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                "https://places.googleapis.com/v1/places:searchText"
            );

            request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            request.Headers.Add("X-Goog-Api-Key", _apiKey);
            request.Headers.Add("X-Goog-FieldMask", "places.photos");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);

            var root = doc.RootElement;
            if (!root.TryGetProperty("places", out var places) || places.GetArrayLength() == 0)
                throw new Exception("No places found.");

            var photos = places[0].GetProperty("photos");
            if (photos.GetArrayLength() == 0)
                throw new Exception("No photos available.");

            var photoName = photos[0].GetProperty("name").GetString();

            var photoUrl = $"https://places.googleapis.com/v1/{photoName}/media?maxWidthPx={maxWidth}";

            using var photoRequest = new HttpRequestMessage(HttpMethod.Get, photoUrl);
            photoRequest.Headers.Add("X-Goog-Api-Key", _apiKey);

            var photoResponse = await _httpClient.SendAsync(photoRequest);
            photoResponse.EnsureSuccessStatusCode();

            return await photoResponse.Content.ReadAsByteArrayAsync();
        }

        public async Task<string> GetPlacesByNameAsync(string query)
        {
            var requestBpdy = JsonSerializer.Serialize(new { textQuery = query });

            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                "https://places.googleapis.com/v1/places:searchText"
            );

            request.Content = new StringContent(requestBpdy, Encoding.UTF8, "application/json");
            request.Headers.Add("X-Goog-Api-Key", _apiKey);
            request.Headers.Add("X-Goog-FieldMask", "places.displayName,places.id");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();

        }

        public async Task<string> GetAutocompletePredictionsAsync(string query)
        {
            var requestBody = JsonSerializer.Serialize(new
            {
                input = query,
                languageCode = "pl"
            });

            using var request = new HttpRequestMessage(
                HttpMethod.Post,
                "https://places.googleapis.com/v1/places:autocomplete"
            );

            request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            request.Headers.Add("X-Goog-Api-Key", _apiKey);
            request.Headers.Add("X-Goog-FieldMask", "suggestions.placePrediction.placeId,suggestions.placePrediction.text");

            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }


    }
}
