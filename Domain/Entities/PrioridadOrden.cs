namespace Domain.Entities;

public class PrioridadOrden : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public int Nivel { get; set; }
    public string? Descripcion { get; set; }
    public bool Activo { get; set; } = true;
    public ICollection<OrdenServicio>? OrdenesServicio { get; set; }
}
