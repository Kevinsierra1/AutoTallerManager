namespace Application.Abstractions;

public interface ICurrentUserService
{
    Guid? UsuarioId { get; }
    string? Email { get; }
    bool EsAutenticado { get; }
    bool TieneRol(string rol);
}
