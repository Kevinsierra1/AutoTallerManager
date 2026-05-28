using MediatR;
using Application.Abstractions;
using Application.Common.Exceptions;

namespace Application.UseCase.Proveedores;

public record DeleteProveedorCommand(Guid Id) : IRequest;

public class DeleteProveedorCommandHandler : IRequestHandler<DeleteProveedorCommand>
{
    private readonly IApplicationDbContext _context;
    public DeleteProveedorCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(DeleteProveedorCommand request, CancellationToken cancellationToken)
    {
        var p = await _context.Proveedores.FindAsync([request.Id], cancellationToken)
            ?? throw new NotFoundException("Proveedor", request.Id);
        p.Eliminado = true;
        p.EliminadoEn = DateTime.UtcNow;
        p.Activo = false;
        await _context.SaveChangesAsync(cancellationToken);
    }
}
