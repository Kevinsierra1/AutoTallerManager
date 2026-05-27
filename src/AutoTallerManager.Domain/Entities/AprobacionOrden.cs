namespace AutoTallerManager.Domain.Entities;

public class AprobacionOrden : BaseEntity
{
    public Guid OrdenServicioId { get; set; }
    public OrdenServicio OrdenServicio { get; set; } = null!;
    public Guid ClienteId { get; set; }
    public Cliente Cliente { get; set; } = null!;
    public DateTime FechaAprobacion { get; set; }
    public bool Aprobada { get; set; }
    public string? Observaciones { get; set; }
}
