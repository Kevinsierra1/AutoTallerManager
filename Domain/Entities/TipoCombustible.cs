namespace Domain.Entities;

public class TipoCombustible : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;
    public ICollection<Vehiculo>? Vehiculos { get; set; }
}
