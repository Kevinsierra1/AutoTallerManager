namespace Application.UseCase.Facturas;

public record FacturaDto(
    Guid Id,
    string NumeroFactura,
    Guid? OrdenServicioId,
    string? NumeroOrden,           // primera orden (compat)
    List<string>? NumerosOrdenes,  // todas las órdenes incluidas
    Guid ClienteId,
    string? ClienteNombre,
    decimal Subtotal,
    decimal Impuestos,
    decimal Descuento,
    decimal Total,
    bool Pagada,
    string? MetodoPago,
    DateTime FechaEmision,
    DateTime CreadoEn
);

// Factura de una sola orden (legado/compatibilidad)
public record GenerarFacturaDto(
    Guid OrdenServicioId,
    decimal Descuento = 0,
    string? MetodoPago = null
);

// Factura consolidada: todas las órdenes finalizadas del cliente
public record GenerarFacturaConsolidadaDto(
    Guid ClienteId,
    decimal Descuento = 0,
    string? MetodoPago = null
);
