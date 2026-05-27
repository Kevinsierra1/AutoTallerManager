using MediatR;
using AutoTallerManager.Application.Common.Interfaces;
using AutoTallerManager.Application.Common.Exceptions;
using AutoTallerManager.Domain.Enums;

namespace AutoTallerManager.Application.Features.Ordenes.Commands;

public record FinalizarOrdenCommand(Guid Id) : IRequest;

public class FinalizarOrdenCommandHandler : IRequestHandler<FinalizarOrdenCommand>
{
    private readonly IApplicationDbContext _context;
    public FinalizarOrdenCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(FinalizarOrdenCommand request, CancellationToken cancellationToken)
    {
        var orden = await _context.OrdenesServicio.FindAsync([request.Id], cancellationToken)
            ?? throw new NotFoundException("OrdenServicio", request.Id);
        orden.Estado = EstadoOrdenEnum.Finalizada;
        orden.FechaFin = DateTime.UtcNow;
        var historial = new Domain.Entities.HistorialEstadoOrden
        {
            OrdenServicioId = orden.Id,
            Estado = EstadoOrdenEnum.Finalizada,
            FechaHora = DateTime.UtcNow
        };
        _context.HistorialEstadosOrden.Add(historial);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
