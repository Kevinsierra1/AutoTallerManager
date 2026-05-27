namespace Domain.Entities;

public class DetalleOrdenServicio : BaseEntity
{
    public Guid OrdenServicioId { get; set; }
    public OrdenServicio OrdenServicio { get; set; } = null!;
    public Guid RepuestoId { get; set; }
    public Repuesto Repuesto { get; set; } = null!;
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public decimal Subtotal => Cantidad * PrecioUnitario;
}
