using AutoTallerManager.Domain.Enums;

namespace AutoTallerManager.Domain.Entities;

public class OrdenServicio : BaseEntity
{
    public string NumeroOrden { get; set; } = string.Empty;
    public Guid ClienteId { get; set; }
    public Cliente Cliente { get; set; } = null!;
    public Guid VehiculoId { get; set; }
    public Vehiculo Vehiculo { get; set; } = null!;
    public Guid? MecanicoId { get; set; }
    public Empleado? Mecanico { get; set; }
    public EstadoOrdenEnum Estado { get; set; } = EstadoOrdenEnum.Pendiente;
    public string? Descripcion { get; set; }
    public DateTime FechaIngreso { get; set; } = DateTime.UtcNow;
    public DateTime? FechaFin { get; set; }
    public decimal? Total { get; set; }
    public ICollection<DetalleOrdenServicio>? DetallesOrdenServicio { get; set; }
    public ICollection<ManoObra>? ManosObra { get; set; }
    public ICollection<HistorialEstadoOrden>? HistorialEstados { get; set; }
    public ICollection<AprobacionOrden>? Aprobaciones { get; set; }
}
