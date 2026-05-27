using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;

namespace Infrastructure.Data.Repositories;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> ObtenerPorIdAsync(Guid id, CancellationToken ct = default) =>
        await _dbSet.FindAsync([id], ct);

    public async Task<IEnumerable<T>> ObtenerTodosAsync(CancellationToken ct = default) =>
        await _dbSet.ToListAsync(ct);

    public async Task<IEnumerable<T>> BuscarAsync(Expression<Func<T, bool>> predicado, CancellationToken ct = default) =>
        await _dbSet.Where(predicado).ToListAsync(ct);

    public async Task AgregarAsync(T entidad, CancellationToken ct = default) =>
        await _dbSet.AddAsync(entidad, ct);

    public void Actualizar(T entidad) => _dbSet.Update(entidad);

    public void Eliminar(T entidad)
    {
        entidad.Eliminado = true;
        entidad.EliminadoEn = DateTime.UtcNow;
        _dbSet.Update(entidad);
    }

    public async Task<bool> ExisteAsync(Expression<Func<T, bool>> predicado, CancellationToken ct = default) =>
        await _dbSet.AnyAsync(predicado, ct);

    public async Task<int> ContarAsync(Expression<Func<T, bool>>? predicado = null, CancellationToken ct = default) =>
        predicado == null ? await _dbSet.CountAsync(ct) : await _dbSet.CountAsync(predicado, ct);

    public IQueryable<T> Query() => _dbSet.AsQueryable();
}
