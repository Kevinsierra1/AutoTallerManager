namespace Domain.Entities;

public class OrdenAreaDetalle : BaseEntity
{
    public Guid OrdenAreaId { get; set; }
    public OrdenArea OrdenArea { get; set; } = null!;
    public Guid RepuestoId { get; set; }
    public Repuesto Repuesto { get; set; } = null!;
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Subtotal { get; set; }
    public string? Observaciones { get; set; }
}
