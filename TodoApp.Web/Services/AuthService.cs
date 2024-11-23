using System.Net.Http.Json;
using TodoApp.Shared.Models;
using TodoApp.Web.Models;

namespace TodoApp.Web.Services;

public interface IAuthService
{
    Task<UserResponse?> LoginAsync(LoginViewModel model);
    Task<UserResponse?> RegisterAsync(RegisterViewModel model);
}

public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public AuthService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<UserResponse?> LoginAsync(LoginViewModel model)
    {
        var loginModel = new UserLogin
        {
            Username = model.Username,
            Password = model.Password
        };

        var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginModel);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<UserResponse>();
        }

        return null;
    }

    public async Task<UserResponse?> RegisterAsync(RegisterViewModel model)
    {
        var registerModel = new UserRegister
        {
            Username = model.Username,
            Email = model.Email,
            Password = model.Password,
            ConfirmPassword = model.ConfirmPassword
        };

        var response = await _httpClient.PostAsJsonAsync("api/auth/register", registerModel);
        if (response.IsSuccessStatusCode)
        {
            return await response.Content.ReadFromJsonAsync<UserResponse>();
        }

        return null;
    }
}
