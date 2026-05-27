using AutoTallerManager.Domain.Enums;

namespace AutoTallerManager.Domain.Entities;

public class HistorialEstadoOrden : BaseEntity
{
    public Guid OrdenServicioId { get; set; }
    public OrdenServicio OrdenServicio { get; set; } = null!;
    public EstadoOrdenEnum Estado { get; set; }
    public string? Observaciones { get; set; }
    public DateTime FechaHora { get; set; } = DateTime.UtcNow;
}
