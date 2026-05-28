namespace Domain.Entities;

public class OrdenAreaManoObra : BaseEntity
{
    public Guid OrdenAreaId { get; set; }
    public OrdenArea OrdenArea { get; set; } = null!;
    public Guid? TecnicoId { get; set; }
    public Empleado? Tecnico { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public decimal HorasTrabajo { get; set; }
    public decimal TarifaHora { get; set; }
    public decimal Total { get; set; }
}
