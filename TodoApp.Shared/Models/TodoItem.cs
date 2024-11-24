using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TodoApp.Shared.Models;

public class TodoItem
{
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required(ErrorMessage = "El título es requerido")]
    [StringLength(100, ErrorMessage = "El título no puede tener más de 100 caracteres")]
    [DisplayName("Título")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "La descripción es requerida")]
    [DisplayName("Descripción")]
    public string? Description { get; set; }

    [DisplayName("Completado")]
    public bool IsCompleted { get; set; } = false;

    [DisplayName("Fecha de creación")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [DisplayName("Fecha de actualización")]
    public DateTime? UpdatedAt { get; set; }

    [DisplayName("Fecha de completado")]
    public DateTime? CompletedAt { get; set; }

    [DisplayName("Fecha de vencimiento")]
    public DateTime? DueDate { get; set; }

    [DisplayName("Prioridad")]
    public Priority Priority { get; set; }

    // Relación con el usuario
    [DisplayName("Id de usuario")]
    public Guid UserId { get; set; }
    public User? User { get; set; }
}
