namespace AutoTallerManager.Domain.Entities;

public class VehiculoPropietario : BaseEntity
{
    public Guid VehiculoId { get; set; }
    public Vehiculo Vehiculo { get; set; } = null!;
    public Guid ClienteId { get; set; }
    public Cliente Cliente { get; set; } = null!;
    public DateTime FechaInicio { get; set; } = DateTime.UtcNow;
    public DateTime? FechaFin { get; set; }
    public bool Activo { get; set; } = true;
}
