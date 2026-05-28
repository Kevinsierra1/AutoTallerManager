using Domain.Enums;

namespace Application.UseCase.OrdenAreas;

public record OrdenAreaDto(
    Guid Id,
    Guid OrdenServicioId,
    string NumeroOrden,
    Guid AreaTallerId,
    string AreaNombre,
    TipoArea TipoArea,
    Guid? MecanicoId,
    string? MecanicoNombre,
    EstadoMiniOrden Estado,
    DateTime? FechaInicio,
    DateTime? FechaFin,
    decimal TotalMateriales,
    decimal TotalManoObra,
    string? Observaciones,
    DateTime CreadoEn
);

public record CreateOrdenAreaDto(
    Guid OrdenServicioId,
    Guid AreaTallerId,
    Guid? MecanicoId,
    string? Observaciones
);

public record AsignarMecanicoAreaDto(
    Guid MecanicoId
);

public record UpdateEstadoAreaDto(
    EstadoMiniOrden NuevoEstado,
    string? Observaciones
);
