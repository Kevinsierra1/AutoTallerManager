namespace Application.UseCase.Notificaciones;

public record NotificacionDto(
    Guid Id,
    string Titulo,
    string Mensaje,
    string Tipo,
    bool Leida,
    DateTime FechaCreacion,
    DateTime? FechaLectura,
    string? Url
);
