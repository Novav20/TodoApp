namespace TodoApp.Shared.Models;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? PasswordHash { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Bio { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? LastLogin { get; set; }
    public int TotalTasks { get; set; }
    public int CompletedTasks { get; set; }
}