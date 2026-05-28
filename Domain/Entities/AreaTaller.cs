using Domain.Enums;

namespace Domain.Entities;

public class AreaTaller : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public TipoArea Tipo { get; set; }
    public string? Descripcion { get; set; }
    public bool Activo { get; set; } = true;
    public ICollection<OrdenArea>? OrdenAreas { get; set; }
}
