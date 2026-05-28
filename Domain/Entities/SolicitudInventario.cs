using Domain.Enums;

namespace Domain.Entities;

public class SolicitudInventario : BaseEntity
{
    public string NumeroSolicitud { get; set; } = string.Empty;
    public Guid? SolicitanteId { get; set; }
    public Usuario? Solicitante { get; set; }
    public Guid? OrdenServicioId { get; set; }
    public OrdenServicio? OrdenServicio { get; set; }
    public Guid? MiniOrdenId { get; set; }
    public MiniOrden? MiniOrden { get; set; }
    public EstadoSolicitudInventario Estado { get; set; } = EstadoSolicitudInventario.Pendiente;
    public Guid? AprobadoPorId { get; set; }
    public string? AprobadoPorNombre { get; set; }
    public DateTime? FechaAprobacion { get; set; }
    public DateTime? FechaAtencion { get; set; }
    public string? Observaciones { get; set; }
    public string? MotivoRechazo { get; set; }
    public ICollection<SolicitudInventarioDetalle>? Detalles { get; set; }
}
