using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using WebAPI.DTOs;
using WebAPI.Services;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TokenController : ControllerBase
    {
        private readonly TokenService _tokenService;
        private readonly UserService _userService;

        public TokenController(TokenService tokenService, UserService userService)
        {
            _tokenService = tokenService;
            _userService = userService;
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto dto)
        {
            if (string.IsNullOrEmpty(dto.RefreshToken))
                return BadRequest(new { reason = "MissingRefreshToken" });

            var result = await _userService.TryRefreshAsync(dto.RefreshToken);

            if (!result.Success)
                return Unauthorized(new { reason = result.Reason });

            return Ok(new
            {
                AccessToken = result.AccessToken,
                RefreshToken = result.RefreshToken
            });
        }
    }
}
