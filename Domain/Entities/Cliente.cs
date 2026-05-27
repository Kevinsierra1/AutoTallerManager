namespace Domain.Entities;

public class Cliente : BaseEntity
{
    /// <summary>Número secuencial amigable (1, 2, 3…). Generado automáticamente por PostgreSQL.</summary>
    public int Numero { get; set; }

    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public Guid? TipoDocumentoId { get; set; }
    public TipoDocumento? TipoDocumento { get; set; }
    public string NumeroDocumento { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Telefono { get; set; }
    public string? Direccion { get; set; }
    public bool Activo { get; set; } = true;
    public ICollection<VehiculoPropietario>? VehiculoPropietarios { get; set; }
    public ICollection<OrdenServicio>? Ordenes { get; set; }
    public ICollection<Factura>? Facturas { get; set; }
}
