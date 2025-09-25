using JWT.Algorithms;
using JWT.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

public class TokenService
{
    private readonly byte[] _secret;
    private readonly UserDb _db;

    public TokenService(IConfiguration configuration, UserDb db)
    {
        _db = db;
        var key = configuration["JwtSettings:Key"];
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException("JWT secret key is missing in configuration.", nameof(configuration));
        _secret = Encoding.UTF8.GetBytes(key);
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

    public async Task<(string AccessToken, string RefreshToken)?> RefreshTokensAsync(string refreshToken)
    {
        var storedToken = await _db.UserRefreshTokens
            .FirstOrDefaultAsync(t => t.RefreshToken == refreshToken);

        if (storedToken == null || storedToken.IsRevoked || storedToken.ExpiryDate < DateTime.UtcNow)
        {
            return null;
        }

        var user = await _db.Users.FindAsync(storedToken.UserId);
        if (user == null)
        {
            return null; 
        }

        storedToken.IsRevoked = true;

        var (newAccessToken, newRefreshToken) = GenerateTokens(user);

        var newRefreshTokenEntity = new UserRefreshToken
        {
            UserId = user.Id,
            RefreshToken = newRefreshToken,
            ExpiryDate = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow,
            IsRevoked = false
        };

        _db.UserRefreshTokens.Add(newRefreshTokenEntity);
        await _db.SaveChangesAsync();

        return (newAccessToken, newRefreshToken);
    }

}