using TodoApp.Shared.Models;

namespace TodoApp.Api.Data
{
    public static class DbSeeder
    {
        public static void Initialize(TodoDbContext context)
        {
            // Asegurarse de que la base de datos está creada
            context.Database.EnsureCreated();

            // Verificar si ya hay datos
            if (context.TodoItems.Any())
            {
                return; // La base de datos ya tiene datos
            }

            // Crear algunos todos de ejemplo
            var todos = new TodoItem[]
            {
                new TodoItem
                {
                    Title = "Completar informe mensual",
                    Description = "Preparar y enviar el informe de ventas del mes",
                    IsCompleted = false,
                    CreatedAt = DateTime.UtcNow.AddDays(-5)
                },
                new TodoItem
                {
                    Title = "Reunión con el equipo",
                    Description = "Discutir los objetivos del próximo trimestre",
                    IsCompleted = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-3),
                    CompletedAt = DateTime.UtcNow.AddDays(-1)
                },
                new TodoItem
                {
                    Title = "Actualizar documentación",
                    Description = "Revisar y actualizar la documentación del proyecto",
                    IsCompleted = false,
                    CreatedAt = DateTime.UtcNow.AddDays(-2)
                },
                new TodoItem
                {
                    Title = "Preparar presentación",
                    Description = "Crear presentación para la reunión con los inversores",
                    IsCompleted = false,
                    CreatedAt = DateTime.UtcNow.AddDays(-1)
                },
                new TodoItem
                {
                    Title = "Revisar correos pendientes",
                    Description = "Responder a los correos importantes y archivar los demás",
                    IsCompleted = true,
                    CreatedAt = DateTime.UtcNow.AddDays(-4),
                    CompletedAt = DateTime.UtcNow.AddHours(-12)
                }
            };

            // Agregar los todos a la base de datos
            context.TodoItems.AddRange(todos);
            context.SaveChanges();
        }
    }
}
