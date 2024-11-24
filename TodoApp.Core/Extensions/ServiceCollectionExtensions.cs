using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TodoApp.Core.Services;
using TodoApp.Core.Settings;
using TodoApp.Shared.Interfaces;

namespace TodoApp.Core.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCoreServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Configure JWT Settings
        var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();
        if (jwtSettings == null)
        {
            throw new InvalidOperationException("JwtSettings configuration is missing");
        }
        services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

        // Add Services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ITodoService, TodoService>();

        return services;
    }
}
