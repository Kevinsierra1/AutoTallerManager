using Domain.Enums;

namespace Domain.Entities;

public class MiniOrdenAprobacion : BaseEntity
{
    public Guid MiniOrdenId { get; set; }
    public MiniOrden MiniOrden { get; set; } = null!;
    public NivelAprobacionMJC Nivel { get; set; }
    public bool Aprobado { get; set; }
    public Guid? AprobadoPorId { get; set; }
    public string? AprobadoPorNombre { get; set; }
    public string? Observacion { get; set; }
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
}
