namespace Domain.Entities;

public class EstadoOrden : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public int Codigo { get; set; }
    public bool Activo { get; set; } = true;
}
