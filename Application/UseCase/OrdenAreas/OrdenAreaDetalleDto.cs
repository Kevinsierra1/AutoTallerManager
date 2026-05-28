using Domain.Enums;

namespace Application.UseCase.OrdenAreas;

public record OrdenAreaDetalleDto(
    Guid Id,
    Guid OrdenServicioId,
    string NumeroOrden,
    Guid AreaTallerId,
    string AreaNombre,
    TipoArea TipoArea,
    Guid? MecanicoId,
    string? MecanicoNombre,
    EstadoMiniOrden Estado,
    string EstadoNombre,
    DateTime? FechaInicio,
    DateTime? FechaFin,
    decimal TotalMateriales,
    decimal TotalManoObra,
    string? Observaciones,
    DateTime CreadoEn,
    List<OrdenAreaDetalleItemDto>? Detalles,
    List<OrdenAreaManoObraDto>? ManosObra
);

public record OrdenAreaDetalleItemDto(
    Guid Id,
    Guid RepuestoId,
    string RepuestoNombre,
    string RepuestoCodigo,
    int Cantidad,
    decimal PrecioUnitario,
    decimal Subtotal,
    string? Observaciones
);

public record OrdenAreaManoObraDto(
    Guid Id,
    string Descripcion,
    decimal HorasTrabajo,
    decimal TarifaHora,
    decimal Total,
    Guid? TecnicoId,
    string? TecnicoNombre
);
