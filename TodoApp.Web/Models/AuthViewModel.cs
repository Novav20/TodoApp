using System.ComponentModel.DataAnnotations;

namespace TodoApp.Web.Models;

public class LoginViewModel
{
    [Required(ErrorMessage = "El nombre de usuario es requerido")]
    [Display(Name = "Usuario")]
    public string? Username { get; set; }

    [Required(ErrorMessage = "La contraseña es requerida")]
    [Display(Name = "Contraseña")]
    [DataType(DataType.Password)]
    public string? Password { get; set; }

    [Display(Name = "Recordarme")]
    public bool RememberMe { get; set; }
}

public class RegisterViewModel
{
    [Required(ErrorMessage = "El nombre de usuario es requerido")]
    [Display(Name = "Usuario")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "El usuario debe tener entre 3 y 50 caracteres")]
    public string? Username { get; set; }

    [Required(ErrorMessage = "El correo electrónico es requerido")]
    [EmailAddress(ErrorMessage = "El correo electrónico no es válido")]
    [Display(Name = "Correo Electrónico")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "La contraseña es requerida")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string? Password { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirmar contraseña")]
    [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
    public string? ConfirmPassword { get; set; }
}
