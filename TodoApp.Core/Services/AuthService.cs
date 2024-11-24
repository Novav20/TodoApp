using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using TodoApp.Shared.Interfaces;
using TodoApp.Shared.Models;
using TodoApp.Shared.Models.Requests;
using TodoApp.Shared.Models.Responses;
using BCrypt.Net;

namespace TodoApp.Core.Services;

public class AuthService : IAuthService
{
    private readonly IUserService _userService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AuthService> _logger;
    private readonly string _jwtSecretKey;
    private readonly string _jwtIssuer;
    private readonly string _jwtAudience;
    private readonly int _jwtExpirationMinutes;

    public AuthService(IUserService userService, IConfiguration configuration, ILogger<AuthService> logger)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _jwtSecretKey = _configuration["JwtSettings:SecretKey"] ?? 
            throw new InvalidOperationException("JWT SecretKey is not configured");
        _jwtIssuer = _configuration["JwtSettings:Issuer"] ?? 
            throw new InvalidOperationException("JWT Issuer is not configured");
        _jwtAudience = _configuration["JwtSettings:Audience"] ?? 
            throw new InvalidOperationException("JWT Audience is not configured");
        _jwtExpirationMinutes = Convert.ToInt32(_configuration["JwtSettings:ExpirationInMinutes"] ?? "60");
    }

    public async Task<AuthResponse?> LoginAsync(UserLogin loginRequest)
    {
        try
        {
            if (loginRequest == null)
            {
                _logger.LogWarning("Login attempt with null request");
                throw new ArgumentNullException(nameof(loginRequest));
            }

            if (string.IsNullOrWhiteSpace(loginRequest.Username) || string.IsNullOrWhiteSpace(loginRequest.Password))
            {
                _logger.LogWarning("Login attempt with empty username or password");
                return null;
            }

            var user = await _userService.GetUserByUsernameAsync(loginRequest.Username);
            if (user == null || string.IsNullOrWhiteSpace(user.PasswordHash))
            {
                _logger.LogWarning("Login attempt for non-existent user: {Username}", loginRequest.Username);
                return null;
            }

            if (!VerifyPasswordHash(loginRequest.Password, user.PasswordHash))
            {
                _logger.LogWarning("Invalid password for user: {Username}", loginRequest.Username);
                return null;
            }

            var token = GenerateJwtToken(user);
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogError("Failed to generate JWT token for user: {Username}", loginRequest.Username);
                return null;
            }

            var userProfile = await _userService.GetUserProfileAsync(user.Id);
            if (userProfile == null)
            {
                _logger.LogError("Failed to get user profile for user: {Username}", loginRequest.Username);
                return null;
            }

            _logger.LogInformation("Successful login for user: {Username}", loginRequest.Username);
            return new AuthResponse
            {
                Id = userProfile.Id,
                Username = userProfile.Username ?? string.Empty,
                Email = userProfile.Email ?? string.Empty,
                FirstName = userProfile.FirstName ?? string.Empty,
                LastName = userProfile.LastName ?? string.Empty,
                CreatedAt = userProfile.CreatedAt,
                UpdatedAt = userProfile.UpdatedAt,
                TotalTasks = userProfile.TotalTasks,
                CompletedTasks = userProfile.CompletedTasks,
                CompletionRate = userProfile.CompletionRate,
                Token = token
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during login process for user: {Username}", loginRequest?.Username);
            throw;
        }
    }

    public async Task<AuthResponse?> RegisterAsync(UserRegister registerRequest)
    {
        try
        {
            if (registerRequest == null)
            {
                _logger.LogWarning("Registration attempt with null request");
                throw new ArgumentNullException(nameof(registerRequest));
            }

            if (string.IsNullOrWhiteSpace(registerRequest.Username) || 
                string.IsNullOrWhiteSpace(registerRequest.Password) ||
                string.IsNullOrWhiteSpace(registerRequest.Email))
            {
                _logger.LogWarning("Registration attempt with empty username, password, or email");
                return null;
            }

            _logger.LogInformation("Attempting to register new user: {Username}", registerRequest.Username);

            var success = await _userService.RegisterUserAsync(registerRequest);
            if (!success)
            {
                _logger.LogWarning("Registration failed for user: {Username}. User might already exist.", registerRequest.Username);
                return null;
            }

            var registeredUser = await _userService.GetUserByUsernameAsync(registerRequest.Username);
            if (registeredUser == null)
            {
                _logger.LogError("User was registered but could not be retrieved: {Username}", registerRequest.Username);
                return null;
            }

            var token = GenerateJwtToken(registeredUser);
            if (string.IsNullOrEmpty(token))
            {
                _logger.LogError("Failed to generate JWT token for new user: {Username}", registerRequest.Username);
                return null;
            }

            var userProfile = await _userService.GetUserProfileAsync(registeredUser.Id);
            if (userProfile == null)
            {
                _logger.LogError("Failed to get user profile for new user: {Username}", registerRequest.Username);
                return null;
            }

            _logger.LogInformation("Successfully registered new user: {Username}", registerRequest.Username);
            return new AuthResponse
            {
                Id = userProfile.Id,
                Username = userProfile.Username ?? string.Empty,
                Email = userProfile.Email ?? string.Empty,
                FirstName = userProfile.FirstName ?? string.Empty,
                LastName = userProfile.LastName ?? string.Empty,
                CreatedAt = userProfile.CreatedAt,
                UpdatedAt = userProfile.UpdatedAt,
                TotalTasks = userProfile.TotalTasks,
                CompletedTasks = userProfile.CompletedTasks,
                CompletionRate = userProfile.CompletionRate,
                Token = token
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during registration process for user: {Username}", registerRequest?.Username);
            throw;
        }
    }

    private string GenerateJwtToken(User user)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSecretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Name, user.Username ?? string.Empty),
                    new Claim(ClaimTypes.Email, user.Email ?? string.Empty)
                }),
                Expires = DateTime.UtcNow.AddMinutes(_jwtExpirationMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _jwtIssuer,
                Audience = _jwtAudience
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating JWT token for user: {Username}", user.Username);
            throw;
        }
    }

    private bool VerifyPasswordHash(string password, string storedHash)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be null or empty.", nameof(password));

        if (string.IsNullOrWhiteSpace(storedHash))
            throw new ArgumentException("Stored hash cannot be null or empty.", nameof(storedHash));

        try
        {
            return BCrypt.Net.BCrypt.Verify(password, storedHash);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Error verifying password hash, attempting legacy format");
            try
            {
                // If the stored hash is in the old format (Base64), create a new BCrypt hash
                var oldHash = Convert.ToBase64String(Encoding.UTF8.GetBytes(password));
                return string.Equals(oldHash, storedHash);
            }
            catch (Exception innerEx)
            {
                _logger.LogError(innerEx, "Error verifying legacy password hash");
                throw;
            }
        }
    }
}
