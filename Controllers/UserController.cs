using WebAPI.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WebAPI.DTOs;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserService _userService;
    private readonly TokenService _tokenService;

    public UsersController(UserService userService, TokenService tokenService)
    {
        _userService = userService;
        _tokenService = tokenService;
    }

    [HttpGet("getAllUsers")]
    [Authorize]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> GetUserById(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user is null)
        {
            return NotFound();
        }
        return user;
    }

    [HttpPost("createUser")]
    public async Task<ActionResult<UserDto>> CreateUser(RegisterRequestDto request)
    {
        var createdUser = await _userService.CreateUserAsync(request);

        return CreatedAtAction(
            nameof(GetUserById),
            new { id = createdUser.Id },
            createdUser
        );
    }

    [HttpPut("updateUser/{id}")]
    public async Task<IActionResult> UpdateUser(int id, UpdateRequestDto request)
    {
        var success = await _userService.UpdateUserAsync(id, request);

        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }


    [HttpDelete("deleteUser/{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var success = await _userService.DeleteUserAsync(id);
        if (!success)
        {
            return NotFound();
        }
        return NoContent();
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto loginDto)
    {
        if (string.IsNullOrWhiteSpace(loginDto.Email) || string.IsNullOrWhiteSpace(loginDto.Password))
        {
            return BadRequest("Email and password are required.");
        }

        var user = await _userService.LoginAsync(loginDto.Email, loginDto.Password);

        if (user is null)
        {
            return Unauthorized("Invalid email or password.");
        }

        return user; 
    }


    [HttpPost("register")]
    public async Task<ActionResult<LoginResponseDto>> Register([FromBody] RegisterRequestDto registerDto)
    {
        if (string.IsNullOrWhiteSpace(registerDto.Username) || string.IsNullOrWhiteSpace(registerDto.Email) || string.IsNullOrWhiteSpace(registerDto.Password))
        {
            return BadRequest("Username, email, and password are required.");
        }

        var user = await _userService.RegisterAsync(registerDto.Username, registerDto.Email, registerDto.Password);

        if (user is null)
        {
            return Conflict("Email is already in use.");
        }

        return user;
    }

}
