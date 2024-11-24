using System.ComponentModel.DataAnnotations;
using TodoApp.Shared.Models.Responses;

namespace TodoApp.Web.Models;

public class UserProfileViewModel
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Username is required")]
    [StringLength(50, ErrorMessage = "Username must be between {2} and {1} characters", MinimumLength = 3)]
    public string? Username { get; set; }

    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string? Email { get; set; }

    [StringLength(50, ErrorMessage = "First name must not exceed {1} characters")]
    public string? FirstName { get; set; }

    [StringLength(50, ErrorMessage = "Last name must not exceed {1} characters")]
    public string? LastName { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public int TotalTasks { get; set; }
    public int CompletedTasks { get; set; }

    public int PendingTasks => TotalTasks - CompletedTasks;
    public double CompletionRate => TotalTasks == 0 ? 0 : ((double)CompletedTasks / TotalTasks) * 100;

    public static UserProfileViewModel FromUserResponse(UserResponse response)
    {
        return new UserProfileViewModel
        {
            Id = response.Id,
            Username = response.Username,
            Email = response.Email,
            FirstName = response.FirstName,
            LastName = response.LastName,
            CreatedAt = response.CreatedAt,
            UpdatedAt = response.UpdatedAt,
            TotalTasks = response.TotalTasks,
            CompletedTasks = response.CompletedTasks
        };
    }
}
