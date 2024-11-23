using TodoApp.Shared.Models;
using TodoApp.Web.Models;

namespace TodoApp.Web.Services
{
    public interface ITodoService
    {
        Task<IEnumerable<TodoItemViewModel>> GetAllTodosAsync();
        Task<TodoItemViewModel> GetTodoByIdAsync(Guid id);
        Task<TodoItemViewModel> CreateTodoAsync(TodoItemViewModel todo);
        Task<TodoItemViewModel> UpdateTodoAsync(TodoItemViewModel todo);
        Task DeleteTodoAsync(Guid id);
        Task<TodoItemViewModel> ToggleCompleteAsync(Guid id);
    }
}
