using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using TodoApp.Shared.Models;
using TodoApp.Web.Models;

namespace TodoApp.Web.Services
{
    public class TodoService : ITodoService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly ILogger<TodoService> _logger;

        public TodoService(HttpClient httpClient, IConfiguration configuration, ILogger<TodoService> logger)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(configuration["ApiSettings:BaseUrl"] ?? throw new InvalidOperationException("API Base URL is not configured"));
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            _logger = logger;
        }

        public async Task<IEnumerable<TodoItemViewModel>> GetAllTodosAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/todo");
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<IEnumerable<TodoItemViewModel>>(_jsonOptions) ?? Enumerable.Empty<TodoItemViewModel>();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error getting all todos");
                throw new ApplicationException("Failed to retrieve todos. Please try again later.", ex);
            }
        }

        public async Task<TodoItemViewModel> GetTodoByIdAsync(Guid id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/todo/{id}");
                response.EnsureSuccessStatusCode();
                var todo = await response.Content.ReadFromJsonAsync<TodoItemViewModel>(_jsonOptions);
                return todo ?? throw new KeyNotFoundException($"Todo item with ID {id} not found");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error getting todo with ID {Id}", id);
                throw new ApplicationException($"Failed to retrieve todo item {id}. Please try again later.", ex);
            }
        }

        public async Task<TodoItemViewModel> CreateTodoAsync(TodoItemViewModel todo)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/todo", todo);
                response.EnsureSuccessStatusCode();
                var createdTodo = await response.Content.ReadFromJsonAsync<TodoItemViewModel>(_jsonOptions);
                return createdTodo ?? throw new InvalidOperationException("Created todo item is null");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error creating todo");
                throw new ApplicationException("Failed to create todo item. Please try again later.", ex);
            }
        }

        public async Task<TodoItemViewModel> UpdateTodoAsync(TodoItemViewModel todo)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/todo/{todo.Id}", todo);
                response.EnsureSuccessStatusCode();
                var updatedTodo = await response.Content.ReadFromJsonAsync<TodoItemViewModel>(_jsonOptions);
                return updatedTodo ?? throw new InvalidOperationException("Updated todo item is null");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error updating todo with ID {Id}", todo.Id);
                throw new ApplicationException($"Failed to update todo item {todo.Id}. Please try again later.", ex);
            }
        }

        public async Task DeleteTodoAsync(Guid id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/todo/{id}");
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error deleting todo with ID {Id}", id);
                throw new ApplicationException($"Failed to delete todo item {id}. Please try again later.", ex);
            }
        }

        public async Task<TodoItemViewModel> ToggleCompleteAsync(Guid id)
        {
            try
            {
                var response = await _httpClient.PutAsync($"api/todo/{id}/toggle", null);
                response.EnsureSuccessStatusCode();
                var updatedTodo = await response.Content.ReadFromJsonAsync<TodoItemViewModel>(_jsonOptions);
                return updatedTodo ?? throw new InvalidOperationException("Updated todo item is null");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error toggling completion status for todo with ID {Id}", id);
                throw new ApplicationException($"Failed to toggle completion status for todo item {id}. Please try again later.", ex);
            }
        }
    }
}
