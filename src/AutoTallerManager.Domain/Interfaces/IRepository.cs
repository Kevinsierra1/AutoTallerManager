using System.Linq.Expressions;
using AutoTallerManager.Domain.Entities;

namespace AutoTallerManager.Domain.Interfaces;

public interface IRepository<T> where T : BaseEntity
{
    Task<T?> ObtenerPorIdAsync(Guid id, CancellationToken ct = default);
    Task<IEnumerable<T>> ObtenerTodosAsync(CancellationToken ct = default);
    Task<IEnumerable<T>> BuscarAsync(Expression<Func<T, bool>> predicado, CancellationToken ct = default);
    Task AgregarAsync(T entidad, CancellationToken ct = default);
    void Actualizar(T entidad);
    void Eliminar(T entidad);
    Task<bool> ExisteAsync(Expression<Func<T, bool>> predicado, CancellationToken ct = default);
    Task<int> ContarAsync(Expression<Func<T, bool>>? predicado = null, CancellationToken ct = default);
    IQueryable<T> Query();
}
