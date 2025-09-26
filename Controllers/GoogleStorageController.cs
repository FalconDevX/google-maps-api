using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebAPI.Services;
using WebAPI.DTOs;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserPlacesController : ControllerBase
    {
        private readonly GoogleStorage _googleStorage;

        public UserPlacesController(GoogleStorage googleStorage)
        {
            _googleStorage = googleStorage;
        }
        [HttpPost("addNewPlace")]
        public async Task<IActionResult> AddUserPlace([FromBody] AddUserPlace dto)
        {
            if (string.IsNullOrWhiteSpace(dto.PlaceName))
                return BadRequest("PlaceName is required");

            await _googleStorage.AddUserPlaceToListFileAsync(dto.UserId, dto.Username, dto.PlaceName);

            return Ok(new
            {
                dto.UserId,
                dto.Username,
                addedPlace = dto.PlaceName,
                message = $"Dodano miejsce '{dto.PlaceName}' do użytkownika {dto.Username}"
            });
        }

        [HttpGet("getPlaces")]
        public async Task<IActionResult> GetUserPlaces([FromQuery] int userId, [FromQuery] string username)
        {
            var content = await _googleStorage.GetUserPlacesFromFileAsync(userId, username);

            if (content == null || content.Count == 0)
            {
                return NotFound($"Brak miejsc dla użytkownika {username}");
            }

            return Ok(new
            {
                UserId = userId,
                Username = username,
                Places = content
            });
        }

        [HttpDelete("deletePlace")]
        public async Task<IActionResult> DeleteUserPlace([FromQuery] int userId, [FromQuery] string username, [FromQuery] string placeName)
        {
            var success = await _googleStorage.DeleteUserPlaceFromFileAsync(userId, username, placeName);

            if (!success)
            {
                return NotFound($"Miejsce '{placeName}' nie znalezione dla użytkownika {username}");
            }
            return Ok(new
            {
                UserId = userId,
                Username = username,
                deletedPlace = placeName,
                message = $"Usunięto miejsce '{placeName}' dla użytkownika {username}"
            });
        }
    }
}
