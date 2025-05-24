using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TodoApi.Dto;
using TodoApi.Services;

namespace TodoApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserServices _services;

        public UserController(UserServices services)
        {
            _services = services;
        }


        [HttpGet]
        public async Task<ActionResult<UserList>> GetAllUsers(int page = 1, int pageSize = 10, string searchTerm = "")
        {
            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
            }
            else
            {
                searchTerm = "";
            }

            var allUsers = await _services.GetAllUsers();
            var filteredUser = allUsers.Where(u =>
            u.Name.ToLower().Contains(searchTerm) ||
            u.Email.ToLower().Contains(searchTerm)
            );

            var totalCount = filteredUser.Count();
            var users = filteredUser
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(new UserList
            {
                Users = users.Select(u => new UserDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email,
                    Password = u.Password
                }).ToList(),
                TotalCount = totalCount,
                SearchTerm = searchTerm
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDto>> GetById(int id)
        {
            var user = await _services.GetById(id);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }
            return Ok(user);

        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login([FromBody] UserDto userDto)
        {
            var user = await _services.Login(userDto);
            if (user == null)
            {
                return NotFound(new { message = "User not found" });
            }
            var response = new
            {
                success = true,
                message = "Login successful",
                data = user
            };

            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<UserDto>> Create(UserDto userDto)
        {
            if (userDto == null)
            {
                return BadRequest(new { message = "User data is null" });
            }

            var createdUser = await _services.Create(userDto);
            return CreatedAtAction(nameof(GetById), new { id = createdUser.Id }, createdUser);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<UserDto>> Update(int id, [FromBody] UserDto userDto)
        {

            var updatedUser = await _services.Update(id, userDto);
            if (updatedUser == null)
            {
                return NotFound(new { message = "User not found" });
            }

            return Ok(updatedUser);

        }

    }
}