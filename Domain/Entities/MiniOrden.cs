using Domain.Enums;

namespace Domain.Entities;

public class MiniOrden : BaseEntity
{
    public string NumeroMiniOrden { get; set; } = string.Empty;

    // Cliente y vehículo — requeridos desde la creación del presupuesto
    public Guid ClienteId { get; set; }
    public Cliente Cliente { get; set; } = null!;
    public Guid VehiculoId { get; set; }
    public Vehiculo Vehiculo { get; set; } = null!;

    // Tipo de servicio elegido al crear el presupuesto
    public Guid? TipoServicioId { get; set; }
    public TipoServicio? TipoServicio { get; set; }

    // Se llena automáticamente cuando el cliente aprueba el presupuesto
    public Guid? OrdenServicioId { get; set; }
    public OrdenServicio? OrdenServicio { get; set; }
    public Guid? OrdenAreaId { get; set; }
    public OrdenArea? OrdenArea { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public EstadoMiniOrden Estado { get; set; } = EstadoMiniOrden.Borrador;
    public Guid? MecanicoId { get; set; }
    public Empleado? Mecanico { get; set; }
    public Guid? JefeTallerId { get; set; }
    public Empleado? JefeTaller { get; set; }
    public DateTime? FechaAprobacionJefe { get; set; }
    public DateTime? FechaAprobacionCliente { get; set; }
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public decimal TotalMateriales { get; set; }
    public decimal TotalManoObra { get; set; }
    public decimal Total { get; set; }
    public string? Observaciones { get; set; }
    public string? MotivoRechazo { get; set; }
    public bool Eliminado { get; set; } = false;
    public ICollection<MiniOrdenDetalle>? Detalles { get; set; }
    public ICollection<MiniOrdenManoObra>? ManosObra { get; set; }
    public ICollection<MiniOrdenHistorial>? Historial { get; set; }
    public ICollection<MiniOrdenAprobacion>? Aprobaciones { get; set; }
    public ICollection<SolicitudInventario>? Solicitudes { get; set; }
}
