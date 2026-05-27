using Domain.Enums;

namespace Domain.Entities;

public class Empleado : BaseEntity
{
    public string Nombres { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string NumeroDocumento { get; set; } = string.Empty;
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public TipoEmpleadoEnum TipoEmpleado { get; set; }
    public string? Especialidad { get; set; }
    public bool Activo { get; set; } = true;
    public ICollection<OrdenServicio>? OrdenesAsignadas { get; set; }
}
