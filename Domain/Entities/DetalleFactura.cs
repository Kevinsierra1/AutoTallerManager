namespace Domain.Entities;

public class DetalleFactura : BaseEntity
{
    public Guid FacturaId { get; set; }
    public Factura Factura { get; set; } = null!;
    public string Descripcion { get; set; } = string.Empty;
    public decimal Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Descuento { get; set; }
    public decimal Impuesto { get; set; }
    public decimal Subtotal { get; set; }
}
