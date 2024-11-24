namespace TodoApp.Shared.Models.Responses;

public class AuthResponse : UserResponse
{
    public string? Token { get; set; }
}
