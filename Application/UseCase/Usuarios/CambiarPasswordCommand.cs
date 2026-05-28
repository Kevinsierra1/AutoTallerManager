using MediatR;
using Application.Abstractions;
using Application.Common.Exceptions;

namespace Application.UseCase.Usuarios;

public record CambiarPasswordCommand(Guid UsuarioId, string NuevaPassword) : IRequest;

public class CambiarPasswordCommandHandler : IRequestHandler<CambiarPasswordCommand>
{
    private readonly IApplicationDbContext _context;
    public CambiarPasswordCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(CambiarPasswordCommand request, CancellationToken cancellationToken)
    {
        var usuario = await _context.Usuarios.FindAsync([request.UsuarioId], cancellationToken)
            ?? throw new NotFoundException("Usuario", request.UsuarioId);
        usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NuevaPassword);
        usuario.ActualizadoEn = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
    }
}
