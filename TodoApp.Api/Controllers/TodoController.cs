using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoApp.Shared.Interfaces;
using TodoApp.Shared.Models.Requests;

namespace TodoApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TodoController : ControllerBase
{
    private readonly ITodoService _todoService;

    public TodoController(ITodoService todoService)
    {
        _todoService = todoService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllTodos()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var id))
            return Unauthorized();

        var todos = await _todoService.GetAllTodosAsync(id);
        return Ok(todos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTodoById(Guid id)
    {
        var todo = await _todoService.GetTodoByIdAsync(id);
        if (todo == null)
            return NotFound();

        return Ok(todo);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTodo([FromBody] CreateTodoRequest todo)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var id))
            return Unauthorized();

        todo.UserId = id;
        var createdTodo = await _todoService.CreateTodoAsync(todo);
        return CreatedAtAction(nameof(GetTodoById), new { id = createdTodo.Id }, createdTodo);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateTodo(Guid id, [FromBody] UpdateTodoRequest todo)
    {
        todo.Id = id;
        var success = await _todoService.UpdateTodoAsync(todo);
        if (!success)
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTodo(Guid id)
    {
        var success = await _todoService.DeleteTodoAsync(id);
        if (!success)
            return NotFound();

        return NoContent();
    }

    [HttpPost("{id}/toggle")]
    public async Task<IActionResult> ToggleTodoCompletion(Guid id)
    {
        var success = await _todoService.ToggleCompleteAsync(id);
        if (!success)
            return NotFound();

        return NoContent();
    }
}
