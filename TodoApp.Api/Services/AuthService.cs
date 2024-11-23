using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TodoApp.Api.Data;
using TodoApp.Api.Settings;
using TodoApp.Shared.Models;

namespace TodoApp.Api.Services;

public interface IAuthService
{
    Task<UserResponse?> LoginAsync(UserLogin login);
    Task<UserResponse?> RegisterAsync(UserRegister register);
}

public class AuthService : IAuthService
{
    private readonly TodoDbContext _context;
    private readonly JwtSettings _jwtSettings;

    public AuthService(TodoDbContext context, JwtSettings jwtSettings)
    {
        _context = context;
        _jwtSettings = jwtSettings;
    }

    public async Task<UserResponse?> LoginAsync(UserLogin login)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == login.Username);

        if (user == null || !VerifyPasswordHash(login.Password ?? "", user.PasswordHash ?? ""))
        {
            return null;
        }

        user.LastLogin = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        return new UserResponse
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Token = GenerateJwtToken(user)
        };
    }

    public async Task<UserResponse?> RegisterAsync(UserRegister register)
    {
        if (register.Password != register.ConfirmPassword)
        {
            return null;
        }

        if (await _context.Users.AnyAsync(u => u.Username == register.Username))
        {
            return null;
        }

        var user = new User
        {
            Username = register.Username,
            Email = register.Email,
            PasswordHash = HashPassword(register.Password ?? ""),
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return new UserResponse
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Token = GenerateJwtToken(user)
        };
    }

    private string GenerateJwtToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username ?? ""),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }

    private bool VerifyPasswordHash(string password, string storedHash)
    {
        var hashedPassword = HashPassword(password);
        return hashedPassword == storedHash;
    }
}
