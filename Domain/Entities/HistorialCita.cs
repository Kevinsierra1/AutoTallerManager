using Domain.Enums;

namespace Domain.Entities;

public class HistorialCita : BaseEntity
{
    public Guid CitaId { get; set; }
    public Cita Cita { get; set; } = null!;
    public EstadoCitaEnum EstadoAnterior { get; set; }
    public EstadoCitaEnum EstadoNuevo { get; set; }
    public string? Observaciones { get; set; }
    public DateTime FechaCambio { get; set; } = DateTime.UtcNow;
    public string? CambiadoPor { get; set; }
}
