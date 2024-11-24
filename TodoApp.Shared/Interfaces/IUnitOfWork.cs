namespace TodoApp.Shared.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    ITodoRepository Todos { get; }
    Task<int> SaveChangesAsync();
}
