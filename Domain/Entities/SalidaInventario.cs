namespace Domain.Entities;

public class SalidaInventario : BaseEntity
{
    public Guid RepuestoId { get; set; }
    public Repuesto Repuesto { get; set; } = null!;
    public int Cantidad { get; set; }
    public string Motivo { get; set; } = string.Empty;
    public DateTime FechaSalida { get; set; } = DateTime.UtcNow;
    public Guid? OrdenServicioId { get; set; }
    public OrdenServicio? OrdenServicio { get; set; }
    public Guid? MovimientoInventarioId { get; set; }
    public MovimientoInventario? MovimientoInventario { get; set; }
    public string? Observaciones { get; set; }
}
