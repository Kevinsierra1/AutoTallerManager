namespace Domain.Entities;

public class SolicitudInventarioDetalle : BaseEntity
{
    public Guid SolicitudInventarioId { get; set; }
    public SolicitudInventario SolicitudInventario { get; set; } = null!;
    public Guid RepuestoId { get; set; }
    public Repuesto Repuesto { get; set; } = null!;
    public int CantidadSolicitada { get; set; }
    public int? CantidadAprobada { get; set; }
    public int? CantidadEntregada { get; set; }
    public string? Observaciones { get; set; }
}
