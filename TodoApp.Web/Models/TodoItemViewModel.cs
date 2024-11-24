using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using TodoApp.Shared.Models;
using TodoApp.Shared.Models.Responses;

namespace TodoApp.Web.Models
{
    public class TodoItemViewModel
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "El título es requerido")]
        [StringLength(100, ErrorMessage = "El título no puede tener más de 100 caracteres")]
        [DisplayName("Título")]
        public string Title { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "La descripción no puede tener más de 500 caracteres")]
        [DisplayName("Descripción")]
        public string? Description { get; set; }

        [DisplayName("Completada")]
        public bool IsCompleted { get; set; }

        [DisplayName("Fecha de vencimiento")]
        public DateTime? DueDate { get; set; }

        [DisplayName("Prioridad")]
        public Priority Priority { get; set; }

        [DisplayName("Fecha de creación")]
        public DateTime CreatedAt { get; set; }

        [DisplayName("Fecha de completado")]
        public DateTime? CompletedAt { get; set; }

        public Guid UserId { get; set; }

        public TodoItemViewModel() { }

        public TodoItemViewModel(TodoItem todo)
        {
            Id = todo.Id;
            Title = todo.Title;
            Description = todo.Description;
            IsCompleted = todo.IsCompleted;
            DueDate = todo.DueDate;
            Priority = todo.Priority;
            CreatedAt = todo.CreatedAt;
            CompletedAt = todo.CompletedAt;
            UserId = todo.UserId;
        }

        public TodoItemDto ToDto()
        {
            return new TodoItemDto
            {
                Id = Id,
                Title = Title,
                Description = Description,
                IsCompleted = IsCompleted,
                DueDate = DueDate,
                Priority = Priority,
                CreatedAt = CreatedAt,
                CompletedAt = CompletedAt,
                UserId = UserId
            };
        }

        public TodoItem ToTodoItem()
        {
            return new TodoItem
            {
                Id = Id,
                Title = Title,
                Description = Description,
                IsCompleted = IsCompleted,
                DueDate = DueDate,
                Priority = Priority,
                CreatedAt = CreatedAt,
                UserId = UserId
            };
        }
    }
}
