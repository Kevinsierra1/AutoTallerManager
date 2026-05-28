using Domain.Enums;

namespace Domain.Entities;

public class OrdenArea : BaseEntity
{
    public Guid OrdenServicioId { get; set; }
    public OrdenServicio OrdenServicio { get; set; } = null!;
    public Guid AreaTallerId { get; set; }
    public AreaTaller AreaTaller { get; set; } = null!;
    public Guid? MecanicoId { get; set; }
    public Empleado? Mecanico { get; set; }
    public EstadoMiniOrden Estado { get; set; } = EstadoMiniOrden.Borrador;
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public string? Observaciones { get; set; }
    public decimal TotalMateriales { get; set; }
    public decimal TotalManoObra { get; set; }
    public ICollection<OrdenAreaDetalle>? Detalles { get; set; }
    public ICollection<OrdenAreaManoObra>? ManosObra { get; set; }
    public ICollection<MiniOrden>? MiniOrdenes { get; set; }
}
