using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TodoApi.Data;
using TodoApi.Dto;
using TodoApi.Models;

namespace TodoApi.Services
{
    public class UserServices
    {
        private readonly MyDbContext _db;

        public UserServices(MyDbContext db)
        {
            _db = db;
        }

        public async Task<List<UserDto>> GetAllUsers()
        {
            var users = await _db.Users
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    Password = u.Password
                })
                .ToListAsync();
            if (users == null || users.Count == 0)
            {
                return new List<UserDto>();
            }
            ;

            return users;
        }

        public async Task<UserDto> GetById(int id)
        {

            var user = await _db.Users
            .Select(u => new UserDto
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                Password = u.Password
            })
            .FirstOrDefaultAsync(u => u.Id == id);

            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            return user;
        }

        public async Task<UserDto> Login(UserDto userDto)
        {
            if (userDto == null)
            {
                throw new ArgumentNullException(nameof(userDto));
            }

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == userDto.Email && u.Password == userDto.Password);
            if (user == null)
            {
                throw new KeyNotFoundException("Invalid email or password");
            }
            var response = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Password = user.Password
            };
            return response;

        }

        public async Task<UserDto> Create(UserDto userDto)
        {
            if (userDto == null)
            {
                throw new ArgumentNullException(nameof(userDto));
            }

            var user = new User
            {
                Name = userDto.Name,
                Email = userDto.Email,
                Password = userDto.Password

            };

            await _db.Users.AddAsync(user);
            await _db.SaveChangesAsync();

            var response = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Password = user.Password
            };

            return response;
        }

        public async Task<UserDto> Update(int id, UserDto userDto)
        {
            if (userDto == null)
            {
                throw new ArgumentNullException(nameof(userDto));
            }

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                throw new KeyNotFoundException("User not found");
            }

            user.Name = userDto.Name;
            user.Email = userDto.Email;
            user.Password = userDto.Password;

            _db.Users.Update(user);
            await _db.SaveChangesAsync();

            var response = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Password = user.Password
            };

            return response;
        }
    }
}