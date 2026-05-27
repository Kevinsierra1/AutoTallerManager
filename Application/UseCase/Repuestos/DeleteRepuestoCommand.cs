using MediatR;
using Application.Abstractions;
using Application.Common.Exceptions;

namespace Application.UseCase.Repuestos;

public record DeleteRepuestoCommand(Guid Id) : IRequest;

public class DeleteRepuestoCommandHandler : IRequestHandler<DeleteRepuestoCommand>
{
    private readonly IApplicationDbContext _context;
    public DeleteRepuestoCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(DeleteRepuestoCommand request, CancellationToken cancellationToken)
    {
        var repuesto = await _context.Repuestos.FindAsync([request.Id], cancellationToken)
            ?? throw new NotFoundException("Repuesto", request.Id);
        repuesto.Eliminado = true;
        repuesto.EliminadoEn = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
    }
}
