using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using WebAPI.DTOs;

namespace WebAPI.Services
{
    public class UserService
    {
        private readonly UserDb _db;
        private readonly TokenService _tokenService;
        private readonly GoogleStorage _googleStorage;

        public UserService(UserDb db, TokenService tokenService, GoogleStorage googleStorage)
        {
            _db = db;
            _tokenService = tokenService;
            _googleStorage = googleStorage;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            return await _db.Users
                .Select(user => new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt
                })
                .ToListAsync();
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            var user = await _db.Users.FindAsync(id);

            var service = new UserRecommendationService();
      
            if (user is null)
            {
                return null;
            }

            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,

                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }

        public async Task<UserDto> CreateUserAsync(RegisterRequestDto dto)
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            var userEntity = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = hashedPassword,
                CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow),
                UpdatedAt = DateOnly.FromDateTime(DateTime.UtcNow)
            };

            _db.Users.Add(userEntity);
            await _db.SaveChangesAsync();

            await _googleStorage.CreateUserFolderAsync(userEntity.Id, userEntity.Username);

            return new UserDto
            {
                Id = userEntity.Id,
                Username = userEntity.Username,
                Email = userEntity.Email,
                CreatedAt = userEntity.CreatedAt,
                UpdatedAt = userEntity.UpdatedAt
            };
        }

        public async Task<bool> UpdateUserAsync(int id, UpdateRequestDto dto)
        {
            var user = await _db.Users.FindAsync(id);
            if (user is null)
            {
                return false;
            }

            user.Username = dto.Username;
            user.Email = dto.Email;

            if (!string.IsNullOrEmpty(dto.Password))
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
            }

            user.UpdatedAt = DateOnly.FromDateTime(DateTime.UtcNow);

            await _db.SaveChangesAsync();
            return true;
        }


        public async Task<bool> DeleteUserAsync(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user is null)
            {
                return false;
            }

            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<LoginResponseDto?> LoginAsync(string email, string password)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null || string.IsNullOrEmpty(user.PasswordHash) ||
                !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return null;
            }

            var oldTokens = _db.UserRefreshTokens.Where(t => t.UserId == user.Id && !t.IsRevoked);
            foreach (var token in oldTokens)
            {
                token.IsRevoked = true;
            }
            await _db.SaveChangesAsync();


            var (accessToken, refreshToken) = _tokenService.GenerateTokens(user);

            var refreshTokenEntity = new UserRefreshToken
            {
                UserId = user.Id,
                RefreshToken = refreshToken,
                ExpiryDate = DateTime.UtcNow.AddDays(7), 
                CreatedAt = DateTime.UtcNow,
                IsRevoked = false
            };

            _db.UserRefreshTokens.Add(refreshTokenEntity);
            await _db.SaveChangesAsync();


            return new LoginResponseDto
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.Username,
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }


        public async Task<LoginResponseDto?> RegisterAsync(string username, string email, string password)
        {
            var existingUser = await _db.Users.FirstOrDefaultAsync(u => u.Email == email || u.Username == username);

            if (existingUser != null)
            {
                throw new ArgumentException("User with this email or username already exists");
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            var user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = hashedPassword,
                CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow),
                UpdatedAt = DateOnly.FromDateTime(DateTime.UtcNow)
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            await _googleStorage.CreateUserFolderAsync(user.Id, user.Username);
            await _googleStorage.CreateUserVotesJsonAsync(user.Id, user.Username);

            var (accessToken, refreshToken) = _tokenService.GenerateTokens(user);

            var refreshTokenEntity = new UserRefreshToken
            {
                UserId = user.Id,
                RefreshToken = refreshToken,
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow,
                IsRevoked = false
            };

            _db.UserRefreshTokens.Add(refreshTokenEntity);
            await _db.SaveChangesAsync();

            return new LoginResponseDto
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.Username,
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public async Task<(bool Success, string Reason, string? AccessToken, string? RefreshToken)> TryRefreshAsync(string refreshToken)
        {
            var storedToken = await _db.UserRefreshTokens
                .FirstOrDefaultAsync(t => t.RefreshToken == refreshToken);

            if (storedToken == null)
                return (false, "RefreshTokenNotFound", null, null);

            if (storedToken.IsRevoked)
                return (false, "RefreshTokenRevoked", null, null);

            if (storedToken.ExpiryDate < DateTime.UtcNow)
                return (false, "RefreshTokenExpired", null, null);

            var user = await _db.Users.FindAsync(storedToken.UserId);
            if (user == null)
                return (false, "UserNotFound", null, null);

            storedToken.IsRevoked = true;

            var (newAccess, newRefresh) = _tokenService.GenerateTokens(user);

            _db.UserRefreshTokens.Add(new UserRefreshToken
            {
                UserId = user.Id,
                RefreshToken = newRefresh,
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                CreatedAt = DateTime.UtcNow
            });

            await _db.SaveChangesAsync();
            return (true, "Success", newAccess, newRefresh);
        }

        public async Task RemoveRevokedRefreshTokensAsync()
        {
            var revokedTokens = await _db.UserRefreshTokens
                .Where(t => t.IsRevoked)
                .ToListAsync();

            _db.UserRefreshTokens.RemoveRange(revokedTokens);
            await _db.SaveChangesAsync();
        }
    }
}