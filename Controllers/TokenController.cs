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

        public TokenController(TokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenDto request)
        {
            if (string.IsNullOrEmpty(request.RefreshToken))
            {
                return BadRequest("Refresh token is required.");
            }

            var result = await _tokenService.RefreshTokensAsync(request.RefreshToken);

            if (result == null)
            {
                return Unauthorized("Invalid or expired refresh token.");
            }

            return Ok(new
            {
                AccessToken = result.Value.AccessToken,
                RefreshToken = result.Value.RefreshToken
            });
        }
    }
}
