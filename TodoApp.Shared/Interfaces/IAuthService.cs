using TodoApp.Shared.Models;
using TodoApp.Shared.Models.Requests;
using TodoApp.Shared.Models.Responses;

namespace TodoApp.Shared.Interfaces;

public interface IAuthService
{
    Task<AuthResponse?> LoginAsync(UserLogin loginRequest);
    Task<AuthResponse?> RegisterAsync(UserRegister registerRequest);
}
