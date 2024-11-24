using System.Text;
using TodoApp.Shared.Interfaces;
using TodoApp.Shared.Models;
using TodoApp.Shared.Models.Requests;
using TodoApp.Shared.Models.Responses;
using BCrypt.Net;

namespace TodoApp.Core.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;

    public UserService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<User?> GetUserByIdAsync(Guid id)
    {
        return await _unitOfWork.Users.GetByIdAsync(id);
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be null or empty.", nameof(username));

        return await _unitOfWork.Users.GetByUsernameAsync(username);
    }

    public async Task<bool> ValidateUserCredentialsAsync(UserLogin loginRequest)
    {
        if (loginRequest == null)
            throw new ArgumentNullException(nameof(loginRequest));

        if (string.IsNullOrWhiteSpace(loginRequest.Username))
            throw new ArgumentException("Username cannot be null or empty.", nameof(loginRequest));

        if (string.IsNullOrWhiteSpace(loginRequest.Password))
            throw new ArgumentException("Password cannot be null or empty.", nameof(loginRequest));

        var user = await _unitOfWork.Users.GetByUsernameAsync(loginRequest.Username);
        if (user == null || string.IsNullOrWhiteSpace(user.PasswordHash)) return false;

        return BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.PasswordHash);
    }

    public async Task<UserResponse?> GetUserProfileAsync(Guid userId)
    {
        if (userId == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty.", nameof(userId));

        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null) return null;

        var totalTasks = await _unitOfWork.Todos.GetTotalTasksCountByUserIdAsync(userId);
        var completedTasks = await _unitOfWork.Todos.GetCompletedTasksCountByUserIdAsync(userId);

        var completionRate = totalTasks > 0 ? (double)completedTasks / totalTasks * 100 : 0;

        return new UserResponse
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            TotalTasks = totalTasks,
            CompletedTasks = completedTasks,
            CompletionRate = completionRate
        };
    }

    public async Task<bool> UpdateUserProfileAsync(Guid userId, UpdateProfileRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        if (user == null) return false;

        if (!string.IsNullOrWhiteSpace(request.Username))
            user.Username = request.Username;
        
        if (!string.IsNullOrWhiteSpace(request.Email))
            user.Email = request.Email;

        if (!string.IsNullOrWhiteSpace(request.FirstName))
            user.FirstName = request.FirstName;

        if (!string.IsNullOrWhiteSpace(request.LastName))
            user.LastName = request.LastName;

        user.UpdatedAt = DateTime.UtcNow;

        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RegisterUserAsync(UserRegister registerRequest)
    {
        if (registerRequest == null)
            throw new ArgumentNullException(nameof(registerRequest));

        if (string.IsNullOrWhiteSpace(registerRequest.Username))
            throw new ArgumentException("Username cannot be null or empty.", nameof(registerRequest));

        if (string.IsNullOrWhiteSpace(registerRequest.Email))
            throw new ArgumentException("Email cannot be null or empty.", nameof(registerRequest));

        if (string.IsNullOrWhiteSpace(registerRequest.Password))
            throw new ArgumentException("Password cannot be null or empty.", nameof(registerRequest));

        // Check if user already exists
        var existingUser = await _unitOfWork.Users.GetByUsernameAsync(registerRequest.Username);
        if (existingUser != null)
            return false;

        // Create new user
        var user = new User
        {
            Username = registerRequest.Username,
            Email = registerRequest.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerRequest.Password),
            CreatedAt = DateTime.UtcNow,
            TotalTasks = 0,
            CompletedTasks = 0
        };

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}
