using Microsoft.EntityFrameworkCore;
using TodoApp.Shared.Models;

namespace TodoApp.Api.Data;

public class TodoDbContext : DbContext
{
    public TodoDbContext(DbContextOptions<TodoDbContext> options)
        : base(options)
    {
    }

    public DbSet<TodoItem> TodoItems { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
            entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");
        });

        modelBuilder.Entity<TodoItem>(entity =>
        {
            entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");
        });
    }
}
