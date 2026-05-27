using Domain.Enums;

namespace Application.UseCase.Empleados;

public record EmpleadoDto(
    Guid Id,
    string Nombres,
    string Apellidos,
    string NumeroDocumento,
    string? Telefono,
    string? Email,
    TipoEmpleadoEnum TipoEmpleado,
    string? Especialidad,
    bool Activo,
    DateTime CreadoEn
);

public record CreateEmpleadoDto(
    string Nombres,
    string Apellidos,
    string NumeroDocumento,
    string? Telefono,
    string? Email,
    TipoEmpleadoEnum TipoEmpleado,
    string? Especialidad
);

public record UpdateEmpleadoDto(
    string? Nombres,
    string? Apellidos,
    string? Telefono,
    string? Email,
    string? Especialidad,
    bool? Activo
);
