using WebAPI.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UserService _userService;

    public UsersController(UserService userService)
    {
        _userService = userService;
    }

    [HttpGet("getAllUsers")]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    [HttpGet("getUserById/{id}")]
    public async Task<ActionResult<UserDto>> GetUserById(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user is null)
        {
            return NotFound();
        }
        return Ok(user);
    }

    [HttpPost("createUser")]
    public async Task<ActionResult<UserDto>> CreateUser(UserDto user)
    {
        var createdUser = await _userService.CreateUserAsync(user);
        return CreatedAtAction(nameof(GetUserById), new { id = createdUser.Id }, createdUser);
    }

    [HttpPut("updateUser/{id}")]
    public async Task<IActionResult> UpdateUser(int id, UserDto inputUser)
    {
        var success = await _userService.UpdateUserAsync(id, inputUser);
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
    public async Task<ActionResult<UserDto>> Login([FromBody] LoginRequestDto loginDto)
    {
        if (string.IsNullOrWhiteSpace(loginDto.Email) || string.IsNullOrWhiteSpace(loginDto.Password))
        {
            return BadRequest("Email and password are required.");
        }

        var user = await _userService.LoginAsync(loginDto.Email, loginDto.Password);

        if(user == null)
        {
            return Unauthorized("Invalid email or password.");
        }

        return Ok(user);
    }

    [HttpPost("register")]

    public async Task<ActionResult<UserDto>> Register([FromBody] RegisterRequestDto registerDto)
    {
        if (string.IsNullOrWhiteSpace(registerDto.Username) || string.IsNullOrWhiteSpace(registerDto.Email) || string.IsNullOrWhiteSpace(registerDto.Password))
        {
            return BadRequest("Username, email, and password are required.");
        }

        var user = await _userService.RegisterAsync(registerDto.Username, registerDto.Email, registerDto.Password);

        if (user == null)
        {
            return Conflict("Email is already in use.");
        }

        return Ok(user);
    }
    
}
