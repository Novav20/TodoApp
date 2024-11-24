using TodoApp.Shared.Models;

namespace TodoApp.Shared.Interfaces;

public interface ITodoRepository : IRepository<TodoItem>
{
    Task<IEnumerable<TodoItem>> GetByUserIdAsync(Guid userId);
    Task<IEnumerable<TodoItem>> GetCompletedByUserIdAsync(Guid userId);
    Task<IEnumerable<TodoItem>> GetPendingByUserIdAsync(Guid userId);
    Task<int> GetTotalTasksCountByUserIdAsync(Guid userId);
    Task<int> GetCompletedTasksCountByUserIdAsync(Guid userId);
}
