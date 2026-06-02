namespace Domain.Entities;

public class Usuario : BaseEntity
{
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public bool Activo { get; set; } = true;
    public ICollection<UsuarioRol> UsuarioRoles { get; set; } = new List<UsuarioRol>();
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public ICollection<SesionUsuario>? Sesiones { get; set; }
    public ICollection<HistorialAcceso>? HistorialAccesos { get; set; }
    public ICollection<Notificacion>? Notificaciones { get; set; }
    public Cliente? Cliente { get; set; }
}
