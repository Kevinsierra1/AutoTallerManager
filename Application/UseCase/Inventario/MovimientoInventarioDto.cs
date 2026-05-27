using Domain.Enums;

namespace Application.UseCase.Inventario;

public record MovimientoInventarioDto(
    Guid Id,
    Guid RepuestoId,
    string? RepuestoNombre,
    TipoMovimientoInventario Tipo,
    int Cantidad,
    int CantidadAnterior,
    int CantidadNueva,
    string? Motivo,
    DateTime FechaMovimiento
);

public record EntradaInventarioDto(
    Guid RepuestoId,
    int Cantidad,
    string? Motivo,
    Guid? ProveedorId
);

public record SalidaInventarioDto(
    Guid RepuestoId,
    int Cantidad,
    string? Motivo,
    Guid? OrdenServicioId
);
