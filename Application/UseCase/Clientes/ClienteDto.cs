using System.ComponentModel.DataAnnotations;

namespace Application.UseCase.Clientes;

public record ClienteDto(
    Guid Id,
    int Numero,
    string Nombres,
    string Apellidos,
    string TipoDocumento,
    string NumeroDocumento,
    string? Email,
    string? Telefono,
    string? Direccion,
    DateTime CreadoEn
);

// Devuelto al crear un cliente — incluye contraseña temporal generada
public record ClienteCreadoDto(
    Guid Id,
    int Numero,
    string Nombres,
    string Apellidos,
    string TipoDocumento,
    string NumeroDocumento,
    string? Email,
    string? Telefono,
    string? Direccion,
    DateTime CreadoEn,
    Guid? UsuarioId,
    string ContrasenaGenerada
);

// Orden resumida dentro del perfil del cliente
public record ResumenOrdenDto(
    Guid Id,
    string NumeroOrden,
    string Estado,
    DateTime FechaIngreso,
    DateTime? FechaFin,
    decimal? Total
);

// Pre-orden pendiente de aprobación (MiniOrden en EnRevisionCliente)
public record ResumenMiniOrdenDto(
    Guid Id,
    string NumeroMiniOrden,
    string Estado,
    string Descripcion,
    decimal Total,
    DateTime CreadoEn
);

// Devuelto por GET /api/clientes/{id} — perfil completo con órdenes y pre-órdenes
public record ClientePerfilDto(
    Guid Id,
    int Numero,
    string Nombres,
    string Apellidos,
    string TipoDocumento,
    string NumeroDocumento,
    string? Email,
    string? Telefono,
    string? Direccion,
    DateTime CreadoEn,
    Guid? UsuarioId,
    List<ResumenOrdenDto>? Ordenes,
    List<ResumenMiniOrdenDto>? PreOrdenesPendientes
);

public record CreateClienteDto(
    string Nombres,
    string Apellidos,
    string TipoDocumento,
    string NumeroDocumento,
    [EmailAddress(ErrorMessage = "El email no tiene un formato válido.")] string Email,
    string? Telefono
);

public record UpdateClienteDto(
    string? Nombres,
    string? Apellidos,
    string? Email,
    string? Telefono,
    string? Direccion
);

public record ClienteFiltroDto(
    string? Busqueda,
    string? TipoDocumento,
    int PageNumber = 1,
    int PageSize = 10
);
