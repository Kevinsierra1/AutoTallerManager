namespace Domain.Entities;

public class DireccionCliente : BaseEntity
{
    public Guid ClienteId { get; set; }
    public Cliente Cliente { get; set; } = null!;
    public string Tipo { get; set; } = string.Empty;
    public string Linea1 { get; set; } = string.Empty;
    public string? Linea2 { get; set; }
    public string Ciudad { get; set; } = string.Empty;
    public string? Departamento { get; set; }
    public string Pais { get; set; } = "Colombia";
    public string? CodigoPostal { get; set; }
    public bool Principal { get; set; } = false;
    public bool Activa { get; set; } = true;
}
