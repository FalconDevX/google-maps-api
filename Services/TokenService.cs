using JWT.Builder;
using JWT.Algorithms;
using System.Text;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;

public class TokenService
{
    private readonly byte[] _secret;
    private readonly UserDb _db;

    public TokenService(IConfiguration configuration, UserDb db)
    {
        _db = db;
        _secret = Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"]);
    }

    public (string AccessToken, string RefreshToken) GenerateTokens(UserDto user)
    {
        var accessToken = new JwtBuilder()
            .WithAlgorithm(new HMACSHA256Algorithm())
            .WithSecret(_secret)
            .AddClaim("exp", DateTimeOffset.UtcNow.AddMinutes(15).ToUnixTimeSeconds())
            .AddClaim("iss", "MyApp")
            .AddClaim("aud", "AppUsers")
            .AddClaim("sub", user.Id)
            .AddClaim("name", user.Username)
            .Encode();

        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
        }
        var refreshToken = Convert.ToBase64String(randomNumber);

        return (accessToken, refreshToken);
    }

    public (string AccessToken, string RefreshToken) GenerateTokens(User user)
    {
        var userDto = new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            PasswordHash = user.PasswordHash,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
        var accessToken = new JwtBuilder()
            .WithAlgorithm(new HMACSHA256Algorithm())
            .WithSecret(_secret)
            .AddClaim("exp", DateTimeOffset.UtcNow.AddMinutes(15).ToUnixTimeSeconds())
            .AddClaim("iss", "MyApp")
            .AddClaim("aud", "AppUsers")
            .AddClaim("sub", userDto.Id)
            .AddClaim("name", userDto.Username)
            .Encode();

        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
        }
        var refreshToken = Convert.ToBase64String(randomNumber);

        return (accessToken, refreshToken);
    }
     public async Task SaveRefreshTokenAsync(UserRefreshToken userRefreshToken)
    {
        _db.UserRefreshTokens.Add(userRefreshToken);
        await _db.SaveChangesAsync();
    }
    
}