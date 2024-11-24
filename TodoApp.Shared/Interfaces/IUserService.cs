using TodoApp.Shared.Models;
using TodoApp.Shared.Models.Requests;
using TodoApp.Shared.Models.Responses;

namespace TodoApp.Shared.Interfaces;

public interface IUserService
{
    Task<User?> GetUserByIdAsync(Guid id);
    Task<User?> GetUserByUsernameAsync(string username);
    Task<bool> ValidateUserCredentialsAsync(UserLogin loginRequest);
    Task<UserResponse?> GetUserProfileAsync(Guid userId);
    Task<bool> UpdateUserProfileAsync(Guid userId, UpdateProfileRequest request);
    Task<bool> RegisterUserAsync(UserRegister registerRequest);
}
