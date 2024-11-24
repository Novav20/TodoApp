using Microsoft.EntityFrameworkCore;
using TodoApp.Shared.Interfaces;
using TodoApp.Shared.Models;
using TodoApp.Shared.Models.Requests;

namespace TodoApp.Core.Services;

public class TodoService : ITodoService
{
    private readonly IUnitOfWork _unitOfWork;

    public TodoService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<TodoItem>> GetAllTodosAsync(Guid userId)
    {
        return await _unitOfWork.Todos.GetByUserIdAsync(userId);
    }

    public async Task<TodoItem?> GetTodoByIdAsync(Guid id)
    {
        return await _unitOfWork.Todos.GetByIdAsync(id);
    }

    public async Task<TodoItem> CreateTodoAsync(CreateTodoRequest todo)
    {
        var todoItem = new TodoItem
        {
            Id = Guid.NewGuid(),
            Title = todo.Title,
            Description = todo.Description,
            DueDate = todo.DueDate,
            Priority = todo.Priority,
            UserId = todo.UserId,
            CreatedAt = DateTime.UtcNow,
            IsCompleted = false
        };

        await _unitOfWork.Todos.AddAsync(todoItem);
        await _unitOfWork.SaveChangesAsync();

        return todoItem;
    }

    public async Task<bool> UpdateTodoAsync(UpdateTodoRequest todo)
    {
        var existingTodo = await _unitOfWork.Todos.GetByIdAsync(todo.Id);
        if (existingTodo == null) return false;

        existingTodo.Title = todo.Title;
        existingTodo.Description = todo.Description;
        existingTodo.DueDate = todo.DueDate;
        existingTodo.Priority = todo.Priority;
        existingTodo.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Todos.Update(existingTodo);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteTodoAsync(Guid id)
    {
        var todo = await _unitOfWork.Todos.GetByIdAsync(id);
        if (todo == null) return false;

        _unitOfWork.Todos.Remove(todo);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ToggleCompleteAsync(Guid id)
    {
        var todo = await _unitOfWork.Todos.GetByIdAsync(id);
        if (todo == null) return false;

        todo.IsCompleted = !todo.IsCompleted;
        todo.CompletedAt = todo.IsCompleted ? DateTime.UtcNow : null;
        todo.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Todos.Update(todo);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}
