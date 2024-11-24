using System.Net.Http.Json;
using TodoApp.Web.Models;
using TodoApp.Shared.Models.Requests;

namespace TodoApp.Web.Services;

public class TodoWebService : ITodoWebService
{
    private readonly HttpClient _httpClient;

    public TodoWebService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IEnumerable<TodoItemViewModel>> GetAllTodosAsync()
    {
        var response = await _httpClient.GetFromJsonAsync<IEnumerable<TodoItemViewModel>>("api/todo");
        return response ?? Array.Empty<TodoItemViewModel>();
    }

    public async Task<TodoItemViewModel> GetTodoByIdAsync(Guid id)
    {
        var response = await _httpClient.GetFromJsonAsync<TodoItemViewModel>($"api/todo/{id}");
        return response ?? new TodoItemViewModel();
    }

    public async Task<TodoItemViewModel> CreateTodoAsync(TodoItemViewModel todo)
    {
        var response = await _httpClient.PostAsJsonAsync("api/todo", todo);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TodoItemViewModel>() ?? new TodoItemViewModel();
    }

    public async Task UpdateTodoAsync(TodoItemViewModel todo)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/todo/{todo.Id}", todo);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteTodoAsync(Guid id)
    {
        var response = await _httpClient.DeleteAsync($"api/todo/{id}");
        response.EnsureSuccessStatusCode();
    }

    public async Task ToggleCompleteAsync(Guid id)
    {
        var response = await _httpClient.PostAsync($"api/todo/{id}/toggle", null);
        response.EnsureSuccessStatusCode();
    }
}
