namespace AutoTallerManager.Domain.Entities;

public class CategoriaRepuesto : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public ICollection<Repuesto> Repuestos { get; set; } = new List<Repuesto>();
}
