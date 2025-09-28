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
                if (User?.Identity == null || !User.Identity.IsAuthenticated)
                {
                    return Unauthorized(new
                    {
                        reason = "User not authenticated",
                        tokenHeader = Request.Headers["Authorization"].ToString()
                    });
                }

                var claims = User.Claims.ToDictionary(c => c.Type, c => c.Value);

                var subClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var nameClaim = User.Identity?.Name ?? "unknown";

                if (string.IsNullOrEmpty(subClaim) || string.IsNullOrEmpty(nameClaim))
                {
                    return BadRequest(new
                    {
                        reason = "Missing expected claims (sub / name)",
                        receivedClaims = claims,
                        tokenHeader = Request.Headers["Authorization"].ToString()
                    });
                }

                if (!int.TryParse(subClaim, out int userId))
                {
                    return BadRequest(new { reason = "Invalid 'sub' claim value", sub = subClaim });
                }

                await _googleStorage.AddUserPlaceToListFileAsync(userId, nameClaim, placeDto);

                return Ok(new
                {
                    message = "Place saved successfully",
                    userId,
                    username = nameClaim,
                    claims
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    reason = "Internal exception",
                    message = ex.Message,
                    stack = ex.StackTrace,
                    tokenHeader = Request.Headers["Authorization"].ToString()
                });
            }
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
