using Domain.Enums;

namespace Application.UseCase.MiniOrdenes;

public record MiniOrdenDto(
    Guid Id,
    string NumeroMiniOrden,
    Guid OrdenServicioId,
    string NumeroOrden,
    Guid? OrdenAreaId,
    string? AreaNombre,
    string Descripcion,
    EstadoMiniOrden Estado,
    string EstadoNombre,
    Guid? MecanicoId,
    string? MecanicoNombre,
    Guid? JefeTallerId,
    string? JefeTallerNombre,
    DateTime? FechaAprobacionJefe,
    DateTime? FechaAprobacionCliente,
    DateTime? FechaInicio,
    DateTime? FechaFin,
    decimal TotalMateriales,
    decimal TotalManoObra,
    decimal Total,
    string? Observaciones,
    string? MotivoRechazo,
    DateTime CreadoEn,
    List<MiniOrdenDetalleDto>? Detalles,
    List<MiniOrdenManoObraDto>? ManosObra
);

public record MiniOrdenDetalleDto(
    Guid Id,
    Guid RepuestoId,
    string RepuestoNombre,
    string RepuestoCodigo,
    int Cantidad,
    decimal PrecioUnitario,
    decimal Subtotal
);

public record MiniOrdenManoObraDto(
    Guid Id,
    string Descripcion,
    decimal HorasTrabajo,
    decimal TarifaHora,
    decimal Total,
    Guid? TecnicoId,
    string? TecnicoNombre
);

public record CreateMiniOrdenDto(
    Guid OrdenServicioId,
    Guid? OrdenAreaId,
    string Descripcion,
    string? Observaciones,
    List<CreateMiniOrdenDetalleDto> Detalles,
    List<CreateMiniOrdenManoObraDto>? ManosObra
);

public record CreateMiniOrdenDetalleDto(
    Guid RepuestoId,
    int Cantidad,
    decimal PrecioUnitario
);

public record CreateMiniOrdenManoObraDto(
    string Descripcion,
    decimal HorasTrabajo,
    decimal TarifaHora,
    Guid? TecnicoId
);

public record AprobarRechazarMiniOrdenDto(
    bool Aprobado,
    string? Observacion
);

public record MiniOrdenFiltroDto(
    Guid? OrdenServicioId,
    EstadoMiniOrden? Estado,
    Guid? MecanicoId,
    int PageNumber = 1,
    int PageSize = 10
);
