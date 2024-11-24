using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TodoApp.Web.Services;
using TodoApp.Shared.Models;
using TodoApp.Shared.Models.Requests;
using TodoApp.Web.Models;

namespace TodoApp.Web.Controllers;

[Authorize]
public class TodoController : Controller
{
    private readonly ITodoWebService _todoService;
    private readonly ILogger<TodoController> _logger;

    public TodoController(ITodoWebService todoService, ILogger<TodoController> logger)
    {
        _todoService = todoService;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            var todos = await _todoService.GetAllTodosAsync();
            return View(todos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting todos");
            TempData["ErrorMessage"] = "Error al obtener las tareas. Por favor, inténtalo de nuevo.";
            return View(Enumerable.Empty<TodoItemViewModel>());
        }
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new TodoItemViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(TodoItemViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            model.UserId = Guid.Parse(userId);
            var todo = await _todoService.CreateTodoAsync(model);
            TempData["SuccessMessage"] = "Tarea creada correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating todo");
            ModelState.AddModelError("", "Error al crear la tarea. Por favor, inténtalo de nuevo.");
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        try
        {
            var todo = await _todoService.GetTodoByIdAsync(id);
            if (todo == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != todo.UserId.ToString())
            {
                return Forbid();
            }

            return View(todo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting todo for edit");
            TempData["ErrorMessage"] = "Error al obtener la tarea. Por favor, inténtalo de nuevo.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(TodoItemViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != model.UserId.ToString())
            {
                return Forbid();
            }

            await _todoService.UpdateTodoAsync(model);
            TempData["SuccessMessage"] = "Tarea actualizada correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating todo");
            ModelState.AddModelError("", "Error al actualizar la tarea. Por favor, inténtalo de nuevo.");
            return View(model);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleComplete(Guid id)
    {
        try
        {
            var todo = await _todoService.GetTodoByIdAsync(id);
            if (todo == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != todo.UserId.ToString())
            {
                return Forbid();
            }

            await _todoService.ToggleCompleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling todo completion");
            TempData["ErrorMessage"] = "Error al actualizar el estado de la tarea. Por favor, inténtalo de nuevo.";
            return RedirectToAction(nameof(Index));
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var todo = await _todoService.GetTodoByIdAsync(id);
            if (todo == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId != todo.UserId.ToString())
            {
                return Forbid();
            }

            await _todoService.DeleteTodoAsync(id);
            TempData["SuccessMessage"] = "Tarea eliminada correctamente.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting todo");
            TempData["ErrorMessage"] = "Error al eliminar la tarea. Por favor, inténtalo de nuevo.";
            return RedirectToAction(nameof(Index));
        }
    }
}
