namespace Domain.Entities;

public class MiniOrdenDetalle : BaseEntity
{
    public Guid MiniOrdenId { get; set; }
    public MiniOrden MiniOrden { get; set; } = null!;
    public Guid RepuestoId { get; set; }
    public Repuesto Repuesto { get; set; } = null!;
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Subtotal { get; set; }
    public string? Observaciones { get; set; }
}
