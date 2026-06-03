using Domain.Enums;

namespace Application.UseCase.MiniOrdenes;

public record MiniOrdenDto(
    Guid Id,
    string NumeroPresupuesto,
    // Cliente y vehículo — base del presupuesto
    Guid ClienteId,
    string ClienteNombre,
    Guid VehiculoId,
    string VehiculoPlaca,
    string VehiculoDescripcion,
    // Orden de servicio — solo existe si el cliente aprobó
    Guid? OrdenServicioId,
    string? NumeroOrden,
    // Área (opcional)
    Guid? OrdenAreaId,
    string? AreaNombre,
    // Descripción y estado
    string Descripcion,
    EstadoMiniOrden Estado,
    string EstadoNombre,
    // Flujo M-J-C
    Guid? MecanicoId,
    string? MecanicoNombre,
    Guid? JefeTallerId,
    string? JefeTallerNombre,
    DateTime? FechaAprobacionJefe,
    DateTime? FechaAprobacionCliente,
    DateTime? FechaInicio,
    DateTime? FechaFin,
    // Totales
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

// ── DTOs de entrada ───────────────────────────────────────────────────────────

public record CreatePresupuestoDto(
    Guid ClienteId,
    Guid VehiculoId,
    string Descripcion,
    string? Observaciones,
    Guid? TipoServicioId,
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
    Guid? ClienteId,
    Guid? VehiculoId,
    Guid? OrdenServicioId,
    EstadoMiniOrden? Estado,
    Guid? MecanicoId,
    int PageNumber = 1,
    int PageSize = 10
);
