namespace AutoTallerManager.Domain.Entities;

public class Proveedor : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string? RazonSocial { get; set; }
    public string? Nit { get; set; }
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public string? Direccion { get; set; }
    public bool Activo { get; set; } = true;
    public ICollection<ProveedorRepuesto>? ProveedorRepuestos { get; set; }
}
