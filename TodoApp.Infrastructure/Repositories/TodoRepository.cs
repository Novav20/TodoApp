using Microsoft.EntityFrameworkCore;
using TodoApp.Infrastructure.Data;
using TodoApp.Shared.Interfaces;
using TodoApp.Shared.Models;

namespace TodoApp.Infrastructure.Repositories;

public class TodoRepository : Repository<TodoItem>, ITodoRepository
{
    public TodoRepository(TodoDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<TodoItem>> GetByUserIdAsync(Guid userId)
    {
        return await _dbSet
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<TodoItem>> GetCompletedByUserIdAsync(Guid userId)
    {
        return await _dbSet
            .Where(t => t.UserId == userId && t.IsCompleted)
            .OrderByDescending(t => t.CompletedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<TodoItem>> GetPendingByUserIdAsync(Guid userId)
    {
        return await _dbSet
            .Where(t => t.UserId == userId && !t.IsCompleted)
            .OrderBy(t => t.DueDate)
            .ToListAsync();
    }

    public async Task<int> GetTotalTasksCountByUserIdAsync(Guid userId)
    {
        return await _dbSet
            .CountAsync(t => t.UserId == userId);
    }

    public async Task<int> GetCompletedTasksCountByUserIdAsync(Guid userId)
    {
        return await _dbSet
            .CountAsync(t => t.UserId == userId && t.IsCompleted);
    }
}
