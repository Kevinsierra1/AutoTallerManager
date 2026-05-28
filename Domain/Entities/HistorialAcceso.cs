namespace Domain.Entities;

public class HistorialAcceso : BaseEntity
{
    public Guid UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;
    public string IpAddress { get; set; } = string.Empty;
    public DateTime FechaAcceso { get; set; } = DateTime.UtcNow;
    public bool Exitoso { get; set; }
    public string? MotivoFallo { get; set; }
    public string? UserAgent { get; set; }
}
