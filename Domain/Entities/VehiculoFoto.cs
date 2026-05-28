namespace Domain.Entities;

public class VehiculoFoto : BaseEntity
{
    public Guid VehiculoId { get; set; }
    public Vehiculo Vehiculo { get; set; } = null!;
    public string UrlFoto { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public DateTime FechaSubida { get; set; } = DateTime.UtcNow;
    public bool Principal { get; set; } = false;
}
