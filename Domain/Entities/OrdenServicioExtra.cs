namespace Domain.Entities;

public class OrdenServicioExtra : BaseEntity
{
    public Guid OrdenServicioId { get; set; }
    public OrdenServicio OrdenServicio { get; set; } = null!;
    public string Descripcion { get; set; } = string.Empty;
    public decimal Costo { get; set; }
    public int Cantidad { get; set; } = 1;
    public decimal Subtotal { get; set; }
}
