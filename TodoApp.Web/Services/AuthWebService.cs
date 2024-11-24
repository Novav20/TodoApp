using System.Net.Http.Json;
using TodoApp.Shared.Models.Requests;
using TodoApp.Shared.Models.Responses;

namespace TodoApp.Web.Services;

public class AuthWebService : IAuthWebService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public AuthWebService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
        _httpClient.BaseAddress = new Uri(_configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5203");
    }

    public async Task<AuthResponse?> LoginAsync(UserLogin loginRequest)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/auth/login", loginRequest);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<AuthResponse>();
            }
            return null;
        }
        catch
        {
            return null;
        }
    }

    public async Task<AuthResponse?> RegisterAsync(UserRegister registerRequest)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("/api/auth/register", registerRequest);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<AuthResponse>();
            }
            return null;
        }
        catch
        {
            return null;
        }
    }
}
