namespace Domain.Entities;

public class Repuesto : BaseEntity
{
    public string Codigo { get; set; } = string.Empty;
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public Guid CategoriaRepuestoId { get; set; }
    public CategoriaRepuesto CategoriaRepuesto { get; set; } = null!;
    public Guid? TipoServicioId { get; set; }
    public TipoServicio? TipoServicio { get; set; }
    public decimal PrecioCompra { get; set; }
    public decimal PrecioVenta { get; set; }
    public int StockActual { get; set; }
    public int StockMinimo { get; set; }
    public int StockCritico { get; set; }
    public string? Unidad { get; set; }
    public string? Ubicacion { get; set; }
    public bool Activo { get; set; } = true;
    public ICollection<MovimientoInventario>? Movimientos { get; set; }
    public ICollection<ProveedorRepuesto>? ProveedorRepuestos { get; set; }
    public ICollection<DetalleOrdenServicio>? DetallesOrden { get; set; }
    public ICollection<HistorialPrecioRepuesto>? HistorialPrecios { get; set; }
    public ICollection<EntradaInventario>? Entradas { get; set; }
    public ICollection<SalidaInventario>? Salidas { get; set; }
    public ICollection<MiniOrdenDetalle>? MiniOrdenDetalles { get; set; }
    public ICollection<OrdenAreaDetalle>? OrdenAreaDetalles { get; set; }
    public ICollection<SolicitudInventarioDetalle>? SolicitudDetalles { get; set; }
    public ICollection<TransferenciaInventarioDetalle>? TransferenciaDetalles { get; set; }
}
