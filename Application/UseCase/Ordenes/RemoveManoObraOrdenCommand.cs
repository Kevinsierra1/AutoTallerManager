using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Abstractions;
using Application.Common.Exceptions;

namespace Application.UseCase.Ordenes;

public record RemoveManoObraOrdenCommand(Guid OrdenId, Guid ManoObraId) : IRequest;

public class RemoveManoObraOrdenCommandHandler : IRequestHandler<RemoveManoObraOrdenCommand>
{
    private readonly IApplicationDbContext _context;

    public RemoveManoObraOrdenCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(RemoveManoObraOrdenCommand request, CancellationToken cancellationToken)
    {
        var mano = await _context.ManosObra
            .FirstOrDefaultAsync(m => m.Id == request.ManoObraId && m.OrdenServicioId == request.OrdenId, cancellationToken)
            ?? throw new NotFoundException("ManoObra", request.ManoObraId);

        _context.ManosObra.Remove(mano);

        // Recalcular total
        var orden = await _context.OrdenesServicio
            .FirstOrDefaultAsync(o => o.Id == request.OrdenId, cancellationToken);

        if (orden != null)
        {
            var subtotalDetalles = await _context.DetallesOrdenServicio
                .Where(d => d.OrdenServicioId == request.OrdenId)
                .SumAsync(d => d.Cantidad * d.PrecioUnitario, cancellationToken);

            var subtotalManosObra = await _context.ManosObra
                .Where(m => m.OrdenServicioId == request.OrdenId && m.Id != request.ManoObraId)
                .SumAsync(m => m.Costo, cancellationToken);

            orden.Total = (subtotalDetalles + subtotalManosObra) > 0
                ? subtotalDetalles + subtotalManosObra
                : (decimal?)null;
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
