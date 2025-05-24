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
    public class TodoController : ControllerBase
    {
        private readonly TodoServices _services;

        public TodoController(TodoServices services)
        {
            _services = services;
        }

        [HttpGet]
        public async Task<ActionResult<TodoList>> GetTodos(int page = 1, int pageSize = 10, string searchTerm = "")
        {
            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
            }
            else
            {
                searchTerm = "";
            }
            // var todos = await _services.Todos
            //     .Where(t => !t.IsDeleted)
            //     .Skip((page - 1) * pageSize)
            //     .Take(pageSize)
            //     .ToListAsync();

            // var totalCount = await services.Todos.CountAsync(t => !t.IsDeleted);
            var allTodos = await _services.GetAllTodos();
            var filteredTodos = allTodos.Where(t =>
                !t.IsDeleted &&
                t.Name.ToLower().Contains(searchTerm) ||
                t.Content.ToLower().Contains(searchTerm));

            var totalCount = filteredTodos.Count();
            var todos = filteredTodos
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();


            return Ok(new TodoList
            {
                Todos = todos.Select(t => new TodoDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Content = t.Content,
                    IsComplete = t.IsComplete,
                    IsDeleted = t.IsDeleted,
                }).ToList(),
                TotalCount = totalCount,
                SearchTerm = searchTerm
            });

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TodoDto>> GetById(int id)
        {
            try
            {
                var todo = await _services.GetById(id);
                if (todo == null)
                {
                    return NotFound();
                }
                return Ok(todo);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<TodoDto>> Create([FromBody] TodoDto todo)
        {
            if (todo == null)
            {
                return BadRequest("Todo cannot be null");
            }

            var createdTodo = await _services.Create(todo);
            return CreatedAtAction(nameof(GetById), new { id = createdTodo.Id }, createdTodo);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TodoDto>> Update(int id, [FromBody] TodoDto todo)
        {
            var updatedDto = await _services.Update(id, todo);
            if (updatedDto == null)
            {
                return NotFound();
            }
            return CreatedAtAction(nameof(GetById), new { id = updatedDto.Id }, updatedDto);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<TodoDto>> Delete(int id)
        {
            var deletedTodo = await _services.Delete(id);
            if (deletedTodo == null)
            {
                return NotFound();
            }
            return Ok(deletedTodo);
        }

    }
}