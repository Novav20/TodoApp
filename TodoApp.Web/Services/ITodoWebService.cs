using TodoApp.Web.Models;

namespace TodoApp.Web.Services;

public interface ITodoWebService
{
    Task<IEnumerable<TodoItemViewModel>> GetAllTodosAsync();
    Task<TodoItemViewModel> GetTodoByIdAsync(Guid id);
    Task<TodoItemViewModel> CreateTodoAsync(TodoItemViewModel todo);
    Task UpdateTodoAsync(TodoItemViewModel todo);
    Task DeleteTodoAsync(Guid id);
    Task ToggleCompleteAsync(Guid id);
}
