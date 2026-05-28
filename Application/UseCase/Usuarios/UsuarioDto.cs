namespace Application.UseCase.Usuarios;

public record UsuarioDto(
    Guid Id,
    string Email,
    string Nombres,
    string Apellidos,
    bool Activo,
    List<string> Roles,
    DateTime CreadoEn
);

public record CreateUsuarioDto(
    string Email,
    string Password,
    string Nombres,
    string Apellidos,
    List<Guid>? RolIds
);

public record UpdateUsuarioDto(
    string? Nombres,
    string? Apellidos,
    string? Email,
    bool? Activo
);

public record CambiarPasswordDto(
    string NuevaPassword
);

public record UsuarioFiltroDto(
    string? Busqueda,
    bool? Activo,
    string? Rol,
    int PageNumber = 1,
    int PageSize = 10
);
