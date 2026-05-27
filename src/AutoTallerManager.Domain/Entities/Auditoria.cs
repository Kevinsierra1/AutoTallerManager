namespace AutoTallerManager.Domain.Entities;

public class Auditoria : BaseEntity
{
    public string Entidad { get; set; } = string.Empty;
    public string RegistroId { get; set; } = string.Empty;
    public string Accion { get; set; } = string.Empty;
    public string? UsuarioId { get; set; }
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
    public string? ValoresAnteriores { get; set; }
    public string? ValoresNuevos { get; set; }
}
