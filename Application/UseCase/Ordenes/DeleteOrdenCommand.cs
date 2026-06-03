using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Abstractions;
using Application.Common.Exceptions;
using Domain.Exceptions;

namespace Application.UseCase.Ordenes;

public record DeleteOrdenCommand(Guid Id) : IRequest;

public class DeleteOrdenCommandHandler : IRequestHandler<DeleteOrdenCommand>
{
    private readonly IApplicationDbContext _context;
    public DeleteOrdenCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(DeleteOrdenCommand request, CancellationToken cancellationToken)
    {
        var orden = await _context.OrdenesServicio
            .FirstOrDefaultAsync(o => o.Id == request.Id && !o.Eliminado, cancellationToken)
            ?? throw new NotFoundException("OrdenServicio", request.Id);

        // No eliminar si ya tiene factura generada
        var tieneFactura = await _context.Facturas
            .AnyAsync(f => f.OrdenServicioId == request.Id, cancellationToken);
        if (tieneFactura)
            throw new DomainException("No se puede eliminar una orden que ya tiene factura generada.");

        orden.Eliminado = true;
        orden.ActualizadoEn = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
    }
}
