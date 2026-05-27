using System.Text.Json;
using AutoTallerManager.Application.Common.Interfaces;
using AutoTallerManager.Domain.Entities;

namespace AutoTallerManager.Infrastructure.Services;

public class AuditoriaService : IAuditoriaService
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public AuditoriaService(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task RegistrarAsync(string entidad, string registroId, string accion,
        string? valoresAnteriores = null, string? valoresNuevos = null, CancellationToken ct = default)
    {
        var auditoria = new Auditoria
        {
            Entidad = entidad,
            RegistroId = registroId,
            Accion = accion,
            UsuarioId = _currentUser.UsuarioId?.ToString(),
            Fecha = DateTime.UtcNow,
            ValoresAnteriores = valoresAnteriores,
            ValoresNuevos = valoresNuevos
        };

        _context.Auditorias.Add(auditoria);
        await _context.SaveChangesAsync(ct);
    }
}
