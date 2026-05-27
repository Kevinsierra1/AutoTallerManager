namespace AutoTallerManager.Domain.Entities;

public abstract class BaseEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public DateTime CreadoEn { get; set; } = DateTime.UtcNow;
    public DateTime? ActualizadoEn { get; set; }
    public bool Eliminado { get; set; } = false;
    public DateTime? EliminadoEn { get; set; }
    public string? CreadoPor { get; set; }
    public string? ActualizadoPor { get; set; }
}
