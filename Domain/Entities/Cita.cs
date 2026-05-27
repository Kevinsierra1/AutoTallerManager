using Domain.Enums;

namespace Domain.Entities;

public class Cita : BaseEntity
{
    public Guid ClienteId { get; set; }
    public Cliente Cliente { get; set; } = null!;
    public Guid VehiculoId { get; set; }
    public Vehiculo Vehiculo { get; set; } = null!;
    public DateTime FechaHora { get; set; }
    public string Motivo { get; set; } = string.Empty;
    public EstadoCitaEnum Estado { get; set; } = EstadoCitaEnum.Pendiente;
    public string? Observaciones { get; set; }
}
