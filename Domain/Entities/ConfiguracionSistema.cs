namespace Domain.Entities;

public class ConfiguracionSistema : BaseEntity
{
    public string Clave { get; set; } = string.Empty;
    public string Valor { get; set; } = string.Empty;
    public string Tipo { get; set; } = "string";
    public string? Descripcion { get; set; }
    public string? Grupo { get; set; }
    public bool Activo { get; set; } = true;
}
