namespace Domain.Entities;

public class EntradaInventario : BaseEntity
{
    public Guid RepuestoId { get; set; }
    public Repuesto Repuesto { get; set; } = null!;
    public Guid? ProveedorId { get; set; }
    public Proveedor? Proveedor { get; set; }
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Total { get; set; }
    public DateTime FechaEntrada { get; set; } = DateTime.UtcNow;
    public string? NumeroFacturaProveedor { get; set; }
    public Guid? MovimientoInventarioId { get; set; }
    public MovimientoInventario? MovimientoInventario { get; set; }
    public string? Observaciones { get; set; }
}
