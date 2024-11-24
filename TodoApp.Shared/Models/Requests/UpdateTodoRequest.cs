using System.ComponentModel.DataAnnotations;
using TodoApp.Shared.Models;

namespace TodoApp.Shared.Models.Requests;

public class UpdateTodoRequest
{
    public Guid Id { get; set; }

    [Required(ErrorMessage = "El título es requerido")]
    [StringLength(100, ErrorMessage = "El título no puede tener más de 100 caracteres")]
    public string Title { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "La descripción no puede tener más de 500 caracteres")]
    public string? Description { get; set; }

    public DateTime? DueDate { get; set; }

    public Priority Priority { get; set; }

    public bool IsCompleted { get; set; }

    public Guid UserId { get; set; }
}
