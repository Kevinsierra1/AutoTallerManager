namespace Application.UseCase.Roles;

public record RolDto(
    Guid Id,
    string Nombre,
    string? Descripcion,
    List<string> Permisos,
    DateTime CreadoEn
);

public record CreateRolDto(
    string Nombre,
    string? Descripcion
);

public record UpdateRolDto(
    string? Nombre,
    string? Descripcion
);
