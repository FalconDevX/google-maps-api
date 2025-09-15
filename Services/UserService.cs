using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            var userEntity = new User
            {
                Username = userDto.Username,
                Email = userDto.Email,
                PasswordHash = userDto.PasswordHash,
                CreatedAt = DateOnly.FromDateTime(DateTime.UtcNow)
            };

            _db.Users.Add(userEntity);
            await _db.SaveChangesAsync();

            userDto.Id = userEntity.Id;
            userDto.CreatedAt = userEntity.CreatedAt;
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
            user.PasswordHash = inputUser.PasswordHash;
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
    }
}