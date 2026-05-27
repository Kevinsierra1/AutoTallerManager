namespace AutoTallerManager.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRepository<T> Repository<T>() where T : Domain.Entities.BaseEntity;
    Task<int> GuardarCambiosAsync(CancellationToken ct = default);
    Task IniciarTransaccionAsync(CancellationToken ct = default);
    Task ConfirmarTransaccionAsync(CancellationToken ct = default);
    Task RevertirTransaccionAsync(CancellationToken ct = default);
}
