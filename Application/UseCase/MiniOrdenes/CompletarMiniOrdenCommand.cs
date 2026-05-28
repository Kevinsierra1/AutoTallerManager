using MediatR;
using Application.Abstractions;
using Application.Common.Exceptions;
using Domain.Entities;
using Domain.Enums;

namespace Application.UseCase.MiniOrdenes;

public record CompletarMiniOrdenCommand(Guid MiniOrdenId, string? Observacion) : IRequest<MiniOrdenDto>;

public class CompletarMiniOrdenCommandHandler : IRequestHandler<CompletarMiniOrdenCommand, MiniOrdenDto>
{
    private readonly IApplicationDbContext _context;
    public CompletarMiniOrdenCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<MiniOrdenDto> Handle(CompletarMiniOrdenCommand request, CancellationToken cancellationToken)
    {
        var m = await _context.MiniOrdenes.FindAsync([request.MiniOrdenId], cancellationToken)
            ?? throw new NotFoundException("MiniOrden", request.MiniOrdenId);

        if (m.Estado != EstadoMiniOrden.EnProceso)
            throw new InvalidOperationException("Solo se pueden completar mini-órdenes en proceso.");

        var estadoAnterior = m.Estado;
        m.Estado = EstadoMiniOrden.Completada;
        m.FechaFin = DateTime.UtcNow;
        m.ActualizadoEn = DateTime.UtcNow;

        _context.MiniOrdenHistoriales.Add(new MiniOrdenHistorial
        {
            Id = Guid.NewGuid(), MiniOrdenId = m.Id,
            EstadoAnterior = estadoAnterior, EstadoNuevo = EstadoMiniOrden.Completada,
            Observacion = request.Observacion ?? "Mini-orden completada",
            Fecha = DateTime.UtcNow, CreadoEn = DateTime.UtcNow
        });

        await _context.SaveChangesAsync(cancellationToken);
        return await new GetMiniOrdenByIdQueryHandler(_context).Handle(new GetMiniOrdenByIdQuery(m.Id), cancellationToken);
    }
}
