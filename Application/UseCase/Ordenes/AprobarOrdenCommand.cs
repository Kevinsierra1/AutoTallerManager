using MediatR;
using Application.Abstractions;
using Application.Common.Exceptions;
using Domain.Entities;
using Domain.Enums;

namespace Application.UseCase.Ordenes;

public record AprobarOrdenCommand(Guid OrdenId, Guid ClienteId) : IRequest;

public class AprobarOrdenCommandHandler : IRequestHandler<AprobarOrdenCommand>
{
    private readonly IApplicationDbContext _context;
    public AprobarOrdenCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(AprobarOrdenCommand request, CancellationToken cancellationToken)
    {
        var orden = await _context.OrdenesServicio.FindAsync([request.OrdenId], cancellationToken)
            ?? throw new NotFoundException("OrdenServicio", request.OrdenId);

        if (orden.Estado != EstadoOrdenEnum.Pendiente)
            throw new Domain.Exceptions.DomainException("Solo se pueden aprobar órdenes en estado Pendiente.");

        orden.Estado = EstadoOrdenEnum.Aprobada;

        var aprobacion = new AprobacionOrden
        {
            OrdenServicioId = orden.Id,
            ClienteId = request.ClienteId,
            FechaAprobacion = DateTime.UtcNow,
            Aprobada = true
        };
        _context.AprobacionesOrden.Add(aprobacion);

        var historial = new HistorialEstadoOrden
        {
            OrdenServicioId = orden.Id,
            Estado = EstadoOrdenEnum.Aprobada,
            FechaHora = DateTime.UtcNow
        };
        _context.HistorialEstadosOrden.Add(historial);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
