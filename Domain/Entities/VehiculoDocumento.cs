namespace Domain.Entities;

public class VehiculoDocumento : BaseEntity
{
    public Guid VehiculoId { get; set; }
    public Vehiculo Vehiculo { get; set; } = null!;
    public string Tipo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string UrlDocumento { get; set; } = string.Empty;
    public DateTime? FechaVencimiento { get; set; }
    public DateTime FechaSubida { get; set; } = DateTime.UtcNow;
}
