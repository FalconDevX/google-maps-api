public class UserDto
{
    public int Id { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string PasswordHash { get; set; }
    public DateOnly CreatedAt { get; set; }
    public DateOnly UpdatedAt { get; set; }
}
public class LoginRequestDto
{
    public required string Email { get; set; }
    public required string Password { get; set; }

    //public string? Token { get; set; }
}

public class RegisterRequestDto
{
    public  required string Username { get; set; }
    public  required string Email { get; set; }
    public  required string Password { get; set; }
}