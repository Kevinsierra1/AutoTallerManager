namespace Domain.Entities;

public class EstadoFactura : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public bool Activo { get; set; } = true;
    public ICollection<Factura>? Facturas { get; set; }
}
