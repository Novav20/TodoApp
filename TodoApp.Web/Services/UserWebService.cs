using System.Net.Http.Json;
using TodoApp.Web.Models;

namespace TodoApp.Web.Services;

public class UserWebService : IUserWebService
{
    private readonly HttpClient _httpClient;

    public UserWebService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<UserProfileViewModel?> GetUserProfileAsync(Guid userId)
    {
        return await _httpClient.GetFromJsonAsync<UserProfileViewModel>("api/user/profile");
    }

    public async Task<bool> UpdateUserProfileAsync(UserProfileViewModel model)
    {
        var response = await _httpClient.PutAsJsonAsync("api/user/profile", model);
        return response.IsSuccessStatusCode;
    }
}
