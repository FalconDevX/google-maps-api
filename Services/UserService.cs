using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace WebAPI.Services
{
    public class UserService
    {
        private readonly UserDb _db;

        public UserService(UserDb db)
        {
            _db = db;
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            return await _db.Users
                .Select(user => new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    PasswordHash = user.PasswordHash,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt
                })
                .ToListAsync();
        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            var user = await _db.Users.FindAsync(id);

            if (user is null)
            {
                return null;
            }

            return new UserDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                PasswordHash = user.PasswordHash,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }

        public async Task<UserDto> CreateUserAsync(UserDto userDto)
        {
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(userDto.PasswordHash);
            var userEntity = new User
            {
                Username = userDto.Username,
                Email = userDto.Email,
                PasswordHash = hashedPassword,
                CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow),
                UpdatedAt = DateOnly.FromDateTime(DateTime.UtcNow)
            };

            _db.Users.Add(userEntity);
            await _db.SaveChangesAsync();

            userDto.Id = userEntity.Id;
            userDto.CreatedAt = userEntity.CreatedAt;
            userDto.PasswordHash = null; 
            return userDto;
        }

        public async Task<bool> UpdateUserAsync(int id, UserDto inputUser)
        {
            var user = await _db.Users.FindAsync(id);
            if (user is null)
            {
                return false;
            }

            user.Username = inputUser.Username;
            user.Email = inputUser.Email;
            if (!string.IsNullOrEmpty(inputUser.PasswordHash))
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(inputUser.PasswordHash);
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

        public async Task<UserDto?> LoginAsync(string email, string password)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return null;
            }

            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.Username
            };

        }

        public async Task<UserDto?> RegisterAsync(string username, string email, string password)
        {
            return await CreateUserAsync(new UserDto
            {
                Username = username,
                Email = email,
                PasswordHash = password
            });
        }
    }
}