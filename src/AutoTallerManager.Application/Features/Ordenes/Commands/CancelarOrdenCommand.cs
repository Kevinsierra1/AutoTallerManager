using MediatR;
using AutoTallerManager.Application.Common.Interfaces;
using AutoTallerManager.Application.Common.Exceptions;
using AutoTallerManager.Domain.Enums;

namespace AutoTallerManager.Application.Features.Ordenes.Commands;

public record CancelarOrdenCommand(Guid Id, string Motivo) : IRequest;

public class CancelarOrdenCommandHandler : IRequestHandler<CancelarOrdenCommand>
{
    private readonly IApplicationDbContext _context;
    public CancelarOrdenCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(CancelarOrdenCommand request, CancellationToken cancellationToken)
    {
        var orden = await _context.OrdenesServicio.FindAsync([request.Id], cancellationToken)
            ?? throw new NotFoundException("OrdenServicio", request.Id);
        orden.Estado = EstadoOrdenEnum.Cancelada;
        var historial = new Domain.Entities.HistorialEstadoOrden
        {
            OrdenServicioId = orden.Id,
            Estado = EstadoOrdenEnum.Cancelada,
            Observaciones = request.Motivo,
            FechaHora = DateTime.UtcNow
        };
        _context.HistorialEstadosOrden.Add(historial);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
