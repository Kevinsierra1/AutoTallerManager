namespace Domain.Entities;

public class TipoTransmision : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;
    public ICollection<Vehiculo>? Vehiculos { get; set; }
}
