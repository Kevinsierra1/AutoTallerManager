using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Abstractions;
using Application.Common.Exceptions;

namespace Application.UseCase.Ordenes;

public record RemoveDetalleOrdenCommand(Guid OrdenId, Guid DetalleId) : IRequest;

public class RemoveDetalleOrdenCommandHandler : IRequestHandler<RemoveDetalleOrdenCommand>
{
    private readonly IApplicationDbContext _context;

    public RemoveDetalleOrdenCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(RemoveDetalleOrdenCommand request, CancellationToken cancellationToken)
    {
        var detalle = await _context.DetallesOrdenServicio
            .FirstOrDefaultAsync(d => d.Id == request.DetalleId && d.OrdenServicioId == request.OrdenId, cancellationToken)
            ?? throw new NotFoundException("DetalleOrdenServicio", request.DetalleId);

        _context.DetallesOrdenServicio.Remove(detalle);

        // Recalcular total
        var orden = await _context.OrdenesServicio
            .FirstOrDefaultAsync(o => o.Id == request.OrdenId, cancellationToken);

        if (orden != null)
        {
            var subtotalDetalles = await _context.DetallesOrdenServicio
                .Where(d => d.OrdenServicioId == request.OrdenId && d.Id != request.DetalleId)
                .SumAsync(d => d.Cantidad * d.PrecioUnitario, cancellationToken);

            var subtotalManosObra = await _context.ManosObra
                .Where(m => m.OrdenServicioId == request.OrdenId)
                .SumAsync(m => m.Costo, cancellationToken);

            orden.Total = (subtotalDetalles + subtotalManosObra) > 0
                ? subtotalDetalles + subtotalManosObra
                : (decimal?)null;
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
