using TodoApp.Shared.Models.Requests;
using TodoApp.Shared.Models.Responses;

namespace TodoApp.Web.Services;

public interface IAuthWebService
{
    Task<AuthResponse?> LoginAsync(UserLogin loginRequest);
    Task<AuthResponse?> RegisterAsync(UserRegister registerRequest);
}
