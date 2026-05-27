using Domain.Enums;

namespace Application.UseCase.Ordenes;

public record OrdenServicioDto(
    Guid Id,
    string NumeroOrden,
    Guid ClienteId,
    string? ClienteNombre,
    Guid VehiculoId,
    string? VehiculoPlaca,
    Guid? MecanicoId,
    string? MecanicoNombre,
    EstadoOrdenEnum Estado,
    string? Descripcion,
    DateTime FechaIngreso,
    DateTime? FechaFin,
    decimal? Total,
    DateTime CreadoEn
);

public record CreateOrdenDto(
    Guid ClienteId,
    Guid VehiculoId,
    string? Descripcion,
    Guid? TipoServicioId
);

public record OrdenFiltroDto(
    Guid? ClienteId,
    Guid? VehiculoId,
    EstadoOrdenEnum? Estado,
    DateTime? FechaDesde,
    DateTime? FechaHasta,
    int PageNumber = 1,
    int PageSize = 10
);
