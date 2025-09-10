using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Threading.Tasks;
using WebAPI.Services;

namespace WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoogleMapsController : ControllerBase
    {
        private readonly GoogleMapsService _mapsService;

        public GoogleMapsController(GoogleMapsService mapsService)
        {
            _mapsService = mapsService;
        }

        [HttpGet("searchPlaceByName")]
        public async Task<IActionResult> searchPlaceByName([FromQuery] string query)
        {
            var json = await _mapsService.GetPlacePropertiesByNameAsync(query);

            return Ok(JsonDocument.Parse(json).RootElement.GetProperty("places"));
        }

        [HttpGet("getPlaceCoordinatesByName")]
        public async Task<IActionResult> GetPlaceCoordinatesByName([FromQuery] string query)
        {
            var json = await _mapsService.GetPlaceCoordinatesByNameAsync(query);

            using var document = JsonDocument.Parse(json);
            var root = document.RootElement;

            if (!root.TryGetProperty("places", out var places) || places.GetArrayLength() == 0)
            {
                return NotFound("Nie znaleziono miejsca.");
            }

            var location = places[0].GetProperty("location");

            return Ok(new 
            {
                latitude = location.GetProperty("latitude").GetDouble(),
                longitude = location.GetProperty("longitude").GetDouble()
            });
        }

        [HttpGet("getNearbyPlacesByName")]
        public async Task<IActionResult> GetNearbyPlacesByName([FromQuery] string query, [FromQuery] int radius, [FromQuery] string rankPreference)
        {
            var json = await _mapsService.GetNearbyPlacesByCoordinatesAsync(query, radius, rankPreference);
            return Ok(JsonDocument.Parse(json).RootElement.GetProperty("places"));
        }

        [HttpGet("getPlacePhotoByName")]
        public async Task<IActionResult> GetPlacePhotoByName([FromQuery] string query, [FromQuery] int maxWidth = 400)
        {
            var photoBytes = await _mapsService.GetPlacePhotoByNameAsync(query, maxWidth);
            return File(photoBytes, "image/jpeg");
        }

        [HttpGet("GetPlacesByStringAsync")]
        public async Task<IActionResult> GetPlacesByName([FromQuery] string query)
        {
            var json = await _mapsService.GetPlacesByNameAsync(query);
            return Ok(JsonDocument.Parse(json).RootElement.GetProperty("places"));
        }

        [HttpGet("getAutocompletePredictions")]
        public async Task<IActionResult> GetAutocompletePredictions([FromQuery] string query)
        {
            var json = await _mapsService.GetAutocompletePredictionsAsync(query);
            var jsonDoc = JsonDocument.Parse(json);

            if (jsonDoc.RootElement.TryGetProperty("suggestions", out var suggestions) && suggestions.GetArrayLength() > 0)
            {
                return Ok(suggestions);
            }
            return Ok(new object[0]);
        }

    }
}
