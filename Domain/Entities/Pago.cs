namespace Domain.Entities;

public class Pago : BaseEntity
{
    public Guid FacturaId { get; set; }
    public Factura Factura { get; set; } = null!;
    public Guid MetodoPagoId { get; set; }
    public MetodoPago MetodoPago { get; set; } = null!;
    public decimal Monto { get; set; }
    public DateTime FechaPago { get; set; } = DateTime.UtcNow;
    public string? Referencia { get; set; }
}
