using TodoApp.Shared.Models;
using TodoApp.Shared.Models.Requests;
using TodoApp.Shared.Models.Responses;

namespace TodoApp.Shared.Interfaces;

public interface ITodoService
{
    Task<IEnumerable<TodoItem>> GetAllTodosAsync(Guid userId);
    Task<TodoItem?> GetTodoByIdAsync(Guid id);
    Task<TodoItem> CreateTodoAsync(CreateTodoRequest todo);
    Task<bool> UpdateTodoAsync(UpdateTodoRequest todo);
    Task<bool> DeleteTodoAsync(Guid id);
    Task<bool> ToggleCompleteAsync(Guid id);
}
