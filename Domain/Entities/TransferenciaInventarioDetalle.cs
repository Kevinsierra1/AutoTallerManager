namespace Domain.Entities;

public class TransferenciaInventarioDetalle : BaseEntity
{
    public Guid TransferenciaInventarioId { get; set; }
    public TransferenciaInventario TransferenciaInventario { get; set; } = null!;
    public Guid RepuestoId { get; set; }
    public Repuesto Repuesto { get; set; } = null!;
    public int Cantidad { get; set; }
    public string? Observaciones { get; set; }
}
