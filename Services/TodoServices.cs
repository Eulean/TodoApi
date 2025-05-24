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
    public class TodoServices
    {
        private readonly MyDbContext _db;

        public TodoServices(MyDbContext db)
        {
            _db = db;
        }

        public async Task<List<TodoDto>> GetAllTodos()
        {
            var todos = await _db.Todos
                .Where(t => t.IsDeleted == false &&
                            t.DeletedAt == null)
                .Select(t => new TodoDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Content = t.Content,
                    IsComplete = t.IsComplete,
                    IsDeleted = t.IsDeleted
                })
                .ToListAsync();

            if (todos == null || todos.Count == 0)
            {
                return new List<TodoDto>();
            }
            ;

            return todos;
        }

        public async Task<TodoDto> GetById(int id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            var todo = await _db.Todos
               .Where(t => t.IsDeleted == false &&
                           t.DeletedAt == null)
                .Select(t => new TodoDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Content = t.Content,
                    IsComplete = t.IsComplete,
                    IsDeleted = t.IsDeleted
                })
                .FirstOrDefaultAsync(t => t.Id == id);

            if (todo == null)
            {
                throw new KeyNotFoundException("Todo not found");
            }

            return todo;

        }

        public async Task<TodoDto> Create(TodoDto todo)
        {
            if (todo == null)
            {
                throw new ArgumentNullException(nameof(todo));

            }

            var model = new Todo
            {
                Name = todo.Name,
                Content = todo.Content,
                IsComplete = todo.IsComplete,
                CreatedAt = DateTime.UtcNow,

            };

            await _db.Todos.AddAsync(model);
            await _db.SaveChangesAsync();

            var response = new TodoDto
            {
                Id = model.Id,
                Name = model.Name,
                Content = model.Content,
                IsComplete = model.IsComplete,
                IsDeleted = model.IsDeleted
            };

            return response;
        }

        public async Task<TodoDto> Update(int id, TodoDto todo)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            if (todo == null)
            {
                throw new ArgumentNullException(nameof(todo));
            }

            var model = await _db.Todos.FindAsync(id);

            if (model == null)
            {
                throw new KeyNotFoundException("Todo not found");
            }

            model.Name = todo.Name;
            model.Content = todo.Content;
            model.IsComplete = todo.IsComplete;
            model.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            var response = new TodoDto
            {
                Id = model.Id,
                Name = model.Name,
                Content = model.Content,
                IsComplete = model.IsComplete,
                IsDeleted = model.IsDeleted
            };

            return response;
        }

        public async Task<TodoDto> Delete(int id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            var model = await _db.Todos.FindAsync(id);

            if (model == null)
            {
                throw new KeyNotFoundException("Todo not found");
            }

            model.IsDeleted = true;
            model.DeletedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            var response = new TodoDto
            {
                Id = model.Id,
                Name = model.Name,
                Content = model.Content,
                IsComplete = model.IsComplete,
                IsDeleted = model.IsDeleted
            };

            return response;
        }
    }
}