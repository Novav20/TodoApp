using TodoApp.Infrastructure.Repositories;
using TodoApp.Shared.Interfaces;

namespace TodoApp.Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly TodoDbContext _context;
    private IUserRepository? _userRepository;
    private ITodoRepository? _todoRepository;

    public UnitOfWork(TodoDbContext context)
    {
        _context = context;
    }

    public IUserRepository Users => _userRepository ??= new UserRepository(_context);
    public ITodoRepository Todos => _todoRepository ??= new TodoRepository(_context);

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}
