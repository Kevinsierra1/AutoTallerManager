namespace Domain.Entities;

public class MetodoPago : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;
}
