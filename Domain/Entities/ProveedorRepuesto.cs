namespace Domain.Entities;

public class ProveedorRepuesto : BaseEntity
{
    public Guid ProveedorId { get; set; }
    public Proveedor Proveedor { get; set; } = null!;
    public Guid RepuestoId { get; set; }
    public Repuesto Repuesto { get; set; } = null!;
    public decimal? PrecioNegociado { get; set; }
    public bool Activo { get; set; } = true;
}
