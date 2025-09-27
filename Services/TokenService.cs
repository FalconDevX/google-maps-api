using JWT.Algorithms;
using JWT.Builder;
using System.Security.Cryptography;
using System.Text;

public class TokenService
{
    private readonly byte[] _secret;

    public TokenService(IConfiguration configuration)
    {
        var key = configuration["JwtSettings:Key"];
        if (string.IsNullOrEmpty(key))
            throw new ArgumentException("JWT secret key missing.");
        _secret = Encoding.UTF8.GetBytes(key);
    }

    public (string AccessToken, string RefreshToken) GenerateTokens(User user)
    {
        var accessToken = new JwtBuilder()
            .WithAlgorithm(new HMACSHA256Algorithm())
            .WithSecret(_secret)
            .AddClaim("iss", "MyApp")        
            .AddClaim("aud", "AppUsers")    
            .AddClaim("exp", DateTimeOffset.UtcNow.AddMinutes(15).ToUnixTimeSeconds())
            .AddClaim("sub", user.Id)
            .AddClaim("name", user.Username)
            .Encode();

        var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));
        return (accessToken, refreshToken);
    }

}
