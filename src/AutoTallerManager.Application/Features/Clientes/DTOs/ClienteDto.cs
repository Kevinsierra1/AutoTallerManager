namespace AutoTallerManager.Application.Features.Clientes.DTOs;

public record ClienteDto(
    Guid Id,
    string Nombres,
    string Apellidos,
    string TipoDocumento,
    string NumeroDocumento,
    string? Email,
    string? Telefono,
    string? Direccion,
    DateTime CreadoEn
);

public record CreateClienteDto(
    string Nombres,
    string Apellidos,
    string TipoDocumento,
    string NumeroDocumento,
    string? Email,
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
