using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;
using TodoApp.Web.Services;

namespace TodoApp.Web.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWebServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var baseUrl = configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5203";

        // Configure HttpClient for Web Services
        services.AddHttpClient<IAuthWebService, AuthWebService>(client =>
        {
            client.BaseAddress = new Uri(baseUrl);
        });

        services.AddHttpClient<IUserWebService, UserWebService>((sp, client) =>
        {
            client.BaseAddress = new Uri(baseUrl);
            var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
            var token = httpContextAccessor.HttpContext?.User.FindFirst("Token")?.Value;
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        });

        services.AddHttpClient<ITodoWebService, TodoWebService>((sp, client) =>
        {
            client.BaseAddress = new Uri(baseUrl);
            var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
            var token = httpContextAccessor.HttpContext?.User.FindFirst("Token")?.Value;
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        });

        // Add HttpContextAccessor
        services.AddHttpContextAccessor();

        return services;
    }
}
