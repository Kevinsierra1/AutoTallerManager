using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Abstractions;
using Application.Common.Exceptions;

namespace Application.UseCase.MiniOrdenes;

public record RemoveDetalleMiniOrdenCommand(Guid MiniOrdenId, Guid DetalleId) : IRequest;

public class RemoveDetalleMiniOrdenCommandHandler : IRequestHandler<RemoveDetalleMiniOrdenCommand>
{
    private readonly IApplicationDbContext _context;
    public RemoveDetalleMiniOrdenCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(RemoveDetalleMiniOrdenCommand request, CancellationToken cancellationToken)
    {
        var detalle = await _context.MiniOrdenDetalles
            .FirstOrDefaultAsync(d => d.Id == request.DetalleId && d.MiniOrdenId == request.MiniOrdenId, cancellationToken)
            ?? throw new NotFoundException("MiniOrdenDetalle", request.DetalleId);

        _context.MiniOrdenDetalles.Remove(detalle);

        var mini = await _context.MiniOrdenes.FirstOrDefaultAsync(m => m.Id == request.MiniOrdenId, cancellationToken);
        if (mini != null)
        {
            var totalMat = await _context.MiniOrdenDetalles
                .Where(d => d.MiniOrdenId == request.MiniOrdenId && d.Id != request.DetalleId)
                .SumAsync(d => d.Subtotal, cancellationToken);
            mini.TotalMateriales = totalMat;
            mini.Total = totalMat + mini.TotalManoObra;
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
