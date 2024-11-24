using TodoApp.Web.Models;

namespace TodoApp.Web.Services;

public interface IUserWebService
{
    Task<UserProfileViewModel?> GetUserProfileAsync(Guid userId);
    Task<bool> UpdateUserProfileAsync(UserProfileViewModel model);
}
