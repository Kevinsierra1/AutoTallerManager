using Domain.Enums;

namespace Domain.Entities;

public class MiniOrdenHistorial : BaseEntity
{
    public Guid MiniOrdenId { get; set; }
    public MiniOrden MiniOrden { get; set; } = null!;
    public EstadoMiniOrden EstadoAnterior { get; set; }
    public EstadoMiniOrden EstadoNuevo { get; set; }
    public string? Observacion { get; set; }
    public string? CambiadoPor { get; set; }
    public NivelAprobacionMJC? NivelAprobacion { get; set; }
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
}
