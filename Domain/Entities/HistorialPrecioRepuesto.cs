namespace Domain.Entities;

public class HistorialPrecioRepuesto : BaseEntity
{
    public Guid RepuestoId { get; set; }
    public Repuesto Repuesto { get; set; } = null!;
    public decimal PrecioCompraAnterior { get; set; }
    public decimal PrecioCompraNuevo { get; set; }
    public decimal PrecioVentaAnterior { get; set; }
    public decimal PrecioVentaNuevo { get; set; }
    public DateTime FechaCambio { get; set; } = DateTime.UtcNow;
    public string? CambiadoPor { get; set; }
    public string? Motivo { get; set; }
}
