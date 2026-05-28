using MediatR;
using Application.Abstractions;
using Application.Common.Exceptions;

namespace Application.UseCase.Roles;

public record DeleteRolCommand(Guid Id) : IRequest;

public class DeleteRolCommandHandler : IRequestHandler<DeleteRolCommand>
{
    private readonly IApplicationDbContext _context;
    public DeleteRolCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(DeleteRolCommand request, CancellationToken cancellationToken)
    {
        var rol = await _context.Roles.FindAsync([request.Id], cancellationToken)
            ?? throw new NotFoundException("Rol", request.Id);
        rol.Eliminado = true;
        rol.EliminadoEn = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
    }
}
