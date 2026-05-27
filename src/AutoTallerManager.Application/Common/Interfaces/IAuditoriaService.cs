namespace AutoTallerManager.Application.Common.Interfaces;

public interface IAuditoriaService
{
    Task RegistrarAsync(string entidad, string registroId, string accion, string? valoresAnteriores = null, string? valoresNuevos = null, CancellationToken ct = default);
}
