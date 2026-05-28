using Domain.Enums;

namespace Domain.Entities;

public class TransferenciaInventario : BaseEntity
{
    public string NumeroTransferencia { get; set; } = string.Empty;
    public string? Origen { get; set; }
    public string? Destino { get; set; }
    public EstadoSolicitudInventario Estado { get; set; } = EstadoSolicitudInventario.Pendiente;
    public Guid? SolicitadoPorId { get; set; }
    public Usuario? SolicitadoPor { get; set; }
    public Guid? AprobadoPorId { get; set; }
    public string? AprobadoPorNombre { get; set; }
    public DateTime? FechaAprobacion { get; set; }
    public DateTime? FechaCompletado { get; set; }
    public string? Observaciones { get; set; }
    public string? MotivoRechazo { get; set; }
    public ICollection<TransferenciaInventarioDetalle>? Detalles { get; set; }
}
