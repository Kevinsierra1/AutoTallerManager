using MediatR;
using Application.Abstractions;
using Application.Common.Exceptions;

namespace Application.UseCase.Usuarios;

public record DeleteUsuarioCommand(Guid Id) : IRequest;

public class DeleteUsuarioCommandHandler : IRequestHandler<DeleteUsuarioCommand>
{
    private readonly IApplicationDbContext _context;
    public DeleteUsuarioCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(DeleteUsuarioCommand request, CancellationToken cancellationToken)
    {
        var usuario = await _context.Usuarios.FindAsync([request.Id], cancellationToken)
            ?? throw new NotFoundException("Usuario", request.Id);
        usuario.Eliminado = true;
        usuario.EliminadoEn = DateTime.UtcNow;
        usuario.Activo = false;
        await _context.SaveChangesAsync(cancellationToken);
    }
}
