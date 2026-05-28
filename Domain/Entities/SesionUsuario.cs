namespace Domain.Entities;

public class SesionUsuario : BaseEntity
{
    public Guid UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;
    public string IpAddress { get; set; } = string.Empty;
    public string? UserAgent { get; set; }
    public string TokenSesion { get; set; } = string.Empty;
    public DateTime FechaInicio { get; set; } = DateTime.UtcNow;
    public DateTime? FechaFin { get; set; }
    public bool Activa { get; set; } = true;
}
