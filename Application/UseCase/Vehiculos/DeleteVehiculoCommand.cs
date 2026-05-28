using MediatR;
using Application.Abstractions;
using Application.Common.Exceptions;

namespace Application.UseCase.Vehiculos;

public record DeleteVehiculoCommand(Guid Id) : IRequest;

public class DeleteVehiculoCommandHandler : IRequestHandler<DeleteVehiculoCommand>
{
    private readonly IApplicationDbContext _context;
    public DeleteVehiculoCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(DeleteVehiculoCommand request, CancellationToken cancellationToken)
    {
        var v = await _context.Vehiculos.FindAsync([request.Id], cancellationToken)
            ?? throw new NotFoundException("Vehiculo", request.Id);
        v.Eliminado = true;
        v.EliminadoEn = DateTime.UtcNow;
        v.Activo = false;
        await _context.SaveChangesAsync(cancellationToken);
    }
}
