namespace AutoTallerManager.Domain.Entities;

public class TipoServicio : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public decimal? PrecioBase { get; set; }
    public bool Activo { get; set; } = true;
}
