namespace Domain.Entities;

public class Factura : BaseEntity
{
    public string NumeroFactura { get; set; } = string.Empty;
    public Guid OrdenServicioId { get; set; }
    public OrdenServicio OrdenServicio { get; set; } = null!;
    public Guid ClienteId { get; set; }
    public Cliente Cliente { get; set; } = null!;
    public decimal Subtotal { get; set; }
    public decimal Impuestos { get; set; }
    public decimal Descuento { get; set; }
    public decimal Total { get; set; }
    public bool Pagada { get; set; } = false;
    public DateTime FechaEmision { get; set; } = DateTime.UtcNow;
    public ICollection<Pago>? Pagos { get; set; }
}
