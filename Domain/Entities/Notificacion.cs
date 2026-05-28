namespace Domain.Entities;

public class Notificacion : BaseEntity
{
    public Guid? UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Mensaje { get; set; } = string.Empty;
    public string Tipo { get; set; } = "Info";
    public bool Leida { get; set; } = false;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    public DateTime? FechaLectura { get; set; }
    public string? Url { get; set; }
}
