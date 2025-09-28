using System.Security.Claims;


namespace WebAPI.Services
{
    public static class ClaimsUserInfoFromToken
    {
        public static (int userId, string username) GetUserInfo(this ClaimsPrincipal user)
        {
            if (user == null || !user.Identity!.IsAuthenticated)
                throw new UnauthorizedAccessException("User not authenticated");

            var sub = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                       ?? throw new InvalidOperationException("Missing 'sub' claim");

            var name = user.FindFirst("name")?.Value
                       ?? throw new InvalidOperationException("Missing 'name' claim");

            if (!int.TryParse(sub, out int userId))
                throw new InvalidOperationException("Invalid 'sub' claim (not int)");

            return (userId, name);
        }
    }
}
