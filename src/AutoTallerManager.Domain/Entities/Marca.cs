namespace AutoTallerManager.Domain.Entities;

public class Marca : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public ICollection<ModeloVehiculo> Modelos { get; set; } = new List<ModeloVehiculo>();
}
