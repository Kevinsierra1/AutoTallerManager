namespace Application.UseCase.Seguridad;

public record HistorialAccesoDto(
    Guid Id,
    Guid UsuarioId,
    string UsuarioEmail,
    string IpAddress,
    DateTime FechaAcceso,
    bool Exitoso,
    string? MotivoFallo,
    string? UserAgent
);

public record SesionUsuarioDto(
    Guid Id,
    Guid UsuarioId,
    string UsuarioEmail,
    string IpAddress,
    string? UserAgent,
    DateTime FechaInicio,
    DateTime? FechaFin,
    bool Activa
);

public record LogErrorDto(
    Guid Id,
    string Mensaje,
    string? StackTrace,
    string? UsuarioId,
    string? Endpoint,
    DateTime Fecha
);
