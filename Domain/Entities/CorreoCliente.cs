namespace Domain.Entities;

public class CorreoCliente : BaseEntity
{
    public Guid ClienteId { get; set; }
    public Cliente Cliente { get; set; } = null!;
    public string Email { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public bool Principal { get; set; } = false;
    public bool Activo { get; set; } = true;
}
