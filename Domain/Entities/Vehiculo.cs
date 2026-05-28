namespace Domain.Entities;

public class Vehiculo : BaseEntity
{
    public string Placa { get; set; } = string.Empty;
    public string? Vin { get; set; }
    public Guid ModeloVehiculoId { get; set; }
    public ModeloVehiculo ModeloVehiculo { get; set; } = null!;
    public Guid? ColorId { get; set; }
    public Color? Color { get; set; }
    public Guid? TipoCombustibleId { get; set; }
    public TipoCombustible? TipoCombustible { get; set; }
    public Guid? TipoTransmisionId { get; set; }
    public TipoTransmision? TipoTransmision { get; set; }
    public int Anio { get; set; }
    public string? NumeroMotor { get; set; }
    public string? NumeroChasis { get; set; }
    public int KilometrajeActual { get; set; }
    public string? Observaciones { get; set; }
    public bool Activo { get; set; } = true;
    public ICollection<VehiculoPropietario>? Propietarios { get; set; }
    public ICollection<OrdenServicio>? Ordenes { get; set; }
    public ICollection<VehiculoKilometraje>? Kilometrajes { get; set; }
    public ICollection<VehiculoFoto>? Fotos { get; set; }
    public ICollection<VehiculoMantenimiento>? Mantenimientos { get; set; }
    public ICollection<VehiculoDocumento>? Documentos { get; set; }
}
