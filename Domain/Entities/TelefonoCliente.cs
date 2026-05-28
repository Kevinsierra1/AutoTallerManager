namespace Domain.Entities;

public class TelefonoCliente : BaseEntity
{
    public Guid ClienteId { get; set; }
    public Cliente Cliente { get; set; } = null!;
    public string Tipo { get; set; } = string.Empty;
    public string Numero { get; set; } = string.Empty;
    public bool Principal { get; set; } = false;
    public bool Activo { get; set; } = true;
}
