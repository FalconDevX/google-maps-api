using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebAPI.Services;

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

        [HttpPost("{userId}/{username}/places/{placeName}")]
        public async Task<IActionResult> AddPlace(int userId, string username, string placeName)
        {
            if (string.IsNullOrWhiteSpace(placeName))
            {
                return BadRequest("PlaceName is required");
            }

            await _googleStorage.AddUserPlaceToListFile(userId, username, placeName);

            return Ok(new
            {
                userId,
                username,
                addedPlace = placeName,
                message = $"Dodano miejsce '{placeName}' do użytkownika {username}"
            });
        }
    }
}
