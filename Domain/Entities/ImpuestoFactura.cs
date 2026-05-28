namespace Domain.Entities;

public class ImpuestoFactura : BaseEntity
{
    public Guid FacturaId { get; set; }
    public Factura Factura { get; set; } = null!;
    public string Nombre { get; set; } = string.Empty;
    public decimal Porcentaje { get; set; }
    public decimal Base { get; set; }
    public decimal Monto { get; set; }
}
