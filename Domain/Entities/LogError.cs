namespace Domain.Entities;

public class LogError : BaseEntity
{
    public string Mensaje { get; set; } = string.Empty;
    public string? StackTrace { get; set; }
    public string? UsuarioId { get; set; }
    public string? Endpoint { get; set; }
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
}
