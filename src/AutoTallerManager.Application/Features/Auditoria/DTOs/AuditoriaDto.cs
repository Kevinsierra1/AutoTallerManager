namespace AutoTallerManager.Application.Features.Auditoria.DTOs;

public record AuditoriaDto(
    Guid Id,
    string Entidad,
    string RegistroId,
    string Accion,
    string? UsuarioId,
    DateTime Fecha,
    string? ValoresAnteriores,
    string? ValoresNuevos
);

public record AuditoriaFiltroDto(
    string? Entidad,
    string? UsuarioId,
    string? Accion,
    DateTime? FechaDesde,
    DateTime? FechaHasta,
    int PageNumber = 1,
    int PageSize = 10
);
