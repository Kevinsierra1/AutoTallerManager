namespace Domain.Entities;

public class Color : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string? CodigoHex { get; set; }
}
