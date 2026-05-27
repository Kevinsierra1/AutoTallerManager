using AutoTallerManager.Domain.Enums;

namespace AutoTallerManager.Domain.Entities;

public class MovimientoInventario : BaseEntity
{
    public Guid RepuestoId { get; set; }
    public Repuesto Repuesto { get; set; } = null!;
    public TipoMovimientoInventario Tipo { get; set; }
    public int Cantidad { get; set; }
    public int CantidadAnterior { get; set; }
    public int CantidadNueva { get; set; }
    public string? Motivo { get; set; }
    public Guid? ProveedorId { get; set; }
    public Guid? OrdenServicioId { get; set; }
    public DateTime FechaMovimiento { get; set; } = DateTime.UtcNow;
}
