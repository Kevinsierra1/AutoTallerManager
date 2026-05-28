namespace Domain.Entities;

public class Permiso : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string Clave { get; set; } = string.Empty;
    public string? Modulo { get; set; }
    public string? Descripcion { get; set; }
    public bool Activo { get; set; } = true;
    public ICollection<RolPermiso>? RolPermisos { get; set; }
}
