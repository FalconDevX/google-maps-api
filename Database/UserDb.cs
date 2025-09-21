using Microsoft.EntityFrameworkCore;
public class UserDb(DbContextOptions<UserDb> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<UserRefreshToken> UserRefreshTokens => Set<UserRefreshToken>(); 
}