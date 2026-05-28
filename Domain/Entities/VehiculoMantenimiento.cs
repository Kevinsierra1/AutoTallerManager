namespace Domain.Entities;

public class VehiculoMantenimiento : BaseEntity
{
    public Guid VehiculoId { get; set; }
    public Vehiculo Vehiculo { get; set; } = null!;
    public string Tipo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public DateTime FechaMantenimiento { get; set; }
    public DateTime? ProximoMantenimiento { get; set; }
    public int KilometrajeMantenimiento { get; set; }
    public int? ProximoKilometraje { get; set; }
    public Guid? OrdenServicioId { get; set; }
    public OrdenServicio? OrdenServicio { get; set; }
    public decimal Costo { get; set; }
}
