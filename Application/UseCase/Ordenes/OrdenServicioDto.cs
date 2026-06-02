using Domain.Enums;

namespace Application.UseCase.Ordenes;

public record DetalleOrdenDto(
    Guid Id,
    Guid RepuestoId,
    string? RepuestoNombre,
    int Cantidad,
    decimal PrecioUnitario,
    decimal Subtotal
);

public record ManoObraOrdenDto(
    Guid Id,
    string Descripcion,
    decimal Costo,
    decimal HorasTrabajadas,
    Guid? EmpleadoId,
    string? EmpleadoNombre
);

public record CreateDetalleOrdenDto(
    Guid RepuestoId,
    int Cantidad,
    decimal PrecioUnitario
);

public record CreateManoObraOrdenDto(
    string Descripcion,
    decimal HorasTrabajadas,
    decimal Costo,
    Guid? EmpleadoId
);

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
    DateTime CreadoEn,
    List<DetalleOrdenDto>? Detalles = null,
    List<ManoObraOrdenDto>? ManosObra = null
);

public record CreateOrdenDto(
    Guid ClienteId,
    Guid VehiculoId,
    string? Descripcion,
    Guid? TipoServicioId,
    List<CreateDetalleOrdenDto>? Detalles,
    List<CreateManoObraOrdenDto>? ManosObra
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
