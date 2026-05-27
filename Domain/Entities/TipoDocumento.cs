namespace Domain.Entities;

public class TipoDocumento : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string? Abreviatura { get; set; }
}
