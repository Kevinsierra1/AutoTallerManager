namespace Domain.Entities;

public class Factura : BaseEntity
{
    public string NumeroFactura { get; set; } = string.Empty;
    // Nullable — facturas consolidadas no tienen una sola orden principal
    public Guid? OrdenServicioId { get; set; }
    public OrdenServicio? OrdenServicio { get; set; }
    public Guid ClienteId { get; set; }
    public Cliente Cliente { get; set; } = null!;
    public decimal Subtotal { get; set; }
    public decimal Impuestos { get; set; }
    public decimal Descuento { get; set; }
    public decimal Total { get; set; }
    public Guid? EstadoFacturaId { get; set; }
    public EstadoFactura? EstadoFactura { get; set; }
    public bool Pagada { get; set; } = false;
    public string? MetodoPago { get; set; }
    public DateTime FechaEmision { get; set; } = DateTime.UtcNow;
    public ICollection<Pago>? Pagos { get; set; }
    public ICollection<DetalleFactura>? Detalles { get; set; }
    public ICollection<ImpuestoFactura>? FacturaImpuestos { get; set; }
    // Órdenes incluidas en esta factura (consolidada o individual)
    public ICollection<OrdenServicio>? Ordenes { get; set; }
}
