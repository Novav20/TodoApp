using System.ComponentModel.DataAnnotations;

namespace TodoApp.Shared.Models.Requests;

public class UserRegister
{
    [StringLength(50, ErrorMessage = "El nombre no puede tener más de 50 caracteres")]
    public string? FirstName { get; set; }

    [StringLength(50, ErrorMessage = "El apellido no puede tener más de 50 caracteres")]
    public string? LastName { get; set; }

    [Required(ErrorMessage = "El nombre de usuario es requerido")]
    [StringLength(50, ErrorMessage = "El nombre de usuario no puede tener más de 50 caracteres")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "El email es requerido")]
    [EmailAddress(ErrorMessage = "El email no es válido")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es requerida")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener entre 6 y 100 caracteres")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "La confirmación de contraseña es requerida")]
    [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
