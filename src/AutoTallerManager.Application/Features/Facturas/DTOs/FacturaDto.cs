namespace AutoTallerManager.Application.Features.Facturas.DTOs;

public record FacturaDto(
    Guid Id,
    string NumeroFactura,
    Guid OrdenServicioId,
    Guid ClienteId,
    string? ClienteNombre,
    decimal Subtotal,
    decimal Impuestos,
    decimal Descuento,
    decimal Total,
    bool Pagada,
    DateTime FechaEmision,
    DateTime CreadoEn
);

public record GenerarFacturaDto(
    Guid OrdenServicioId,
    decimal Descuento = 0
);
