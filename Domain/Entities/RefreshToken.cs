namespace Domain.Entities;

public class RefreshToken : BaseEntity
{
    public string Token { get; set; } = string.Empty;
    public Guid UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;
    public DateTime Expiracion { get; set; }
    public bool Revocado { get; set; }
    public DateTime? RevokedAt { get; set; }
}
