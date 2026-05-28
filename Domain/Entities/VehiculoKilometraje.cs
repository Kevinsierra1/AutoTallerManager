namespace Domain.Entities;

public class VehiculoKilometraje : BaseEntity
{
    public Guid VehiculoId { get; set; }
    public Vehiculo Vehiculo { get; set; } = null!;
    public int Kilometraje { get; set; }
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
    public string? Observaciones { get; set; }
    public string? RegistradoPor { get; set; }
}
