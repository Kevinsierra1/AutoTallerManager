namespace Domain.Entities;

public class ModeloVehiculo : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public Guid MarcaId { get; set; }
    public Marca Marca { get; set; } = null!;
    public ICollection<Vehiculo> Vehiculos { get; set; } = new List<Vehiculo>();
}
