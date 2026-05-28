namespace Application.UseCase.Proveedores;

public record ProveedorDto(
    Guid Id,
    string Nombre,
    string? RazonSocial,
    string? Nit,
    string? Telefono,
    string? Email,
    string? Direccion,
    bool Activo,
    DateTime CreadoEn
);

public record CreateProveedorDto(
    string Nombre,
    string? RazonSocial,
    string? Nit,
    string? Telefono,
    string? Email,
    string? Direccion
);

public record UpdateProveedorDto(
    string? Nombre,
    string? RazonSocial,
    string? Nit,
    string? Telefono,
    string? Email,
    string? Direccion,
    bool? Activo
);

public record ProveedorFiltroDto(
    string? Busqueda,
    bool? Activo,
    int PageNumber = 1,
    int PageSize = 10
);
