namespace Application.UseCase.Facturas;

public record SolicitudPagoDto(
    Guid Id,
    Guid FacturaId,
    string NumeroFactura,
    string ClienteNombre,
    decimal Monto,
    string TipoPago,
    string Estado,
    string? Token,
    string? Referencia,
    string? Observaciones,
    DateTime FechaSolicitud,
    DateTime? FechaConfirmacion,
    string? ConfirmadoPor
);

public record IniciarPagoDto(
    Guid FacturaId,
    string TipoPago,       // Efectivo | Tarjeta | PSE
    string? Referencia,    // últimos 4 dígitos, banco, etc.
    string? Observaciones
);

public record ConfirmarPagoDto(
    string? Observaciones
);
