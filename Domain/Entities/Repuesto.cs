namespace Domain.Entities;

public class Repuesto : BaseEntity
{
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public Guid CategoriaRepuestoId { get; set; }
    public CategoriaRepuesto CategoriaRepuesto { get; set; } = null!;
    public decimal PrecioCompra { get; set; }
    public decimal PrecioVenta { get; set; }
    public int StockActual { get; set; }
    public int StockMinimo { get; set; }
    public string? Unidad { get; set; }
    public bool Activo { get; set; } = true;
    public ICollection<MovimientoInventario>? Movimientos { get; set; }
    public ICollection<ProveedorRepuesto>? ProveedorRepuestos { get; set; }
    public ICollection<DetalleOrdenServicio>? DetallesOrden { get; set; }
}
