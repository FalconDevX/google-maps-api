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
        [HttpPost]
        public async Task<IActionResult> AddPlace([FromBody] AddUserPlace dto)
        {
            if (string.IsNullOrWhiteSpace(dto.PlaceName))
                return BadRequest("PlaceName is required");

            await _googleStorage.AddUserPlaceToListFile(dto.UserId, dto.Username, dto.PlaceName);

            return Ok(new
            {
                dto.UserId,
                dto.Username,
                addedPlace = dto.PlaceName,
                message = $"Dodano miejsce '{dto.PlaceName}' do użytkownika {dto.Username}"
            });
        }
    }
}
