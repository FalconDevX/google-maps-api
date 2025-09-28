using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebAPI.DTOs;
using WebAPI.Services;
using System.Security.Claims;

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
        [Authorize]
        public async Task<IActionResult> AddNewPlace([FromBody] UserSavedPlacesDto placeDto)
        {
            try
            {
                var (userId, username) = User.GetUserInfo();

                bool added = await _googleStorage.AddUserPlaceToListFileAsync(userId, username, placeDto);

                if (!added)
                {
                    return Conflict(new { message = "Place already exists" }); 
                }

                return Ok(new { message = "Place saved successfully" });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { reason = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { reason = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }


        [HttpGet("getPlaces")]
        [Authorize]
        public async Task<IActionResult> GetUserPlaces()
        {
            try
            {
                var (userId, username) = User.GetUserInfo();

                var places = await _googleStorage.GetUserPlacesFromFileAsync(userId, username);

                if (places == null || !places.Any())
                {
                    return Ok(new
                    {
                        message = "No saved places found",
                        userId,
                        username,
                        places = new List<UserSavedPlacesDto>()
                    });
                }

                return Ok(new
                {
                    message = "Places fetched successfully",
                    userId,
                    username,
                    places
                });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { reason = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { reason = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
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
