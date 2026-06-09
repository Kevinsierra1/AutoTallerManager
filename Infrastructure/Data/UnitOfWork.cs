using Microsoft.EntityFrameworkCore.Storage;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data.Repositories;

namespace Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private readonly Dictionary<Type, object> _repositories = new();
    private IDbContextTransaction? _transaction;

    public UnitOfWork(ApplicationDbContext context) => _context = context;

    public IRepository<T> Repository<T>() where T : BaseEntity
    {
        var type = typeof(T);
        if (!_repositories.TryGetValue(type, out var repo))
        {
            repo = new Repository<T>(_context);
            _repositories[type] = repo;
        }
        return (IRepository<T>)repo;
    }

    public async Task<int> GuardarCambiosAsync(CancellationToken ct = default)
        => await _context.SaveChangesAsync(ct);

    public async Task IniciarTransaccionAsync(CancellationToken ct = default)
        => _transaction = await _context.Database.BeginTransactionAsync(ct);

    public async Task ConfirmarTransaccionAsync(CancellationToken ct = default)
    {
        if (_transaction is null) throw new InvalidOperationException("No hay transacción activa.");
        await _transaction.CommitAsync(ct);
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public async Task RevertirTransaccionAsync(CancellationToken ct = default)
    {
        if (_transaction is null) throw new InvalidOperationException("No hay transacción activa.");
        await _transaction.RollbackAsync(ct);
        await _transaction.DisposeAsync();
        _transaction = null;
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        GC.SuppressFinalize(this);
    }
}
