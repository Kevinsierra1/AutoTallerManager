namespace Domain.Entities;

public class SolicitudPago : BaseEntity
{
    public Guid FacturaId { get; set; }
    public Factura Factura { get; set; } = null!;
    public Guid ClienteId { get; set; }
    public Cliente Cliente { get; set; } = null!;

    // Tipo: Efectivo | Tarjeta | PSE
    public string TipoPago { get; set; } = string.Empty;
    // Estado: Pendiente | Procesando | Confirmada | Rechazada
    public string Estado { get; set; } = "Pendiente";
    // Token generado en pagos electrónicos (tarjeta/PSE)
    public string? Token { get; set; }
    // Referencia adicional (banco PSE, últimos 4 dígitos, etc.)
    public string? Referencia { get; set; }
    public decimal Monto { get; set; }
    public DateTime FechaSolicitud { get; set; } = DateTime.UtcNow;
    public DateTime? FechaConfirmacion { get; set; }
    public string? ConfirmadoPor { get; set; }
    public string? Observaciones { get; set; }
}
