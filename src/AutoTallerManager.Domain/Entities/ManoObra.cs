namespace AutoTallerManager.Domain.Entities;

public class ManoObra : BaseEntity
{
    public Guid OrdenServicioId { get; set; }
    public OrdenServicio OrdenServicio { get; set; } = null!;
    public string Descripcion { get; set; } = string.Empty;
    public decimal Costo { get; set; }
    public decimal HorasTrabajadas { get; set; }
    public Guid? EmpleadoId { get; set; }
    public Empleado? Empleado { get; set; }
}
