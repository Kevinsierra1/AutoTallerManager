using MediatR;
using Application.Abstractions;
using Application.Common.Exceptions;
using Domain.Entities;
using Domain.Enums;

namespace Application.UseCase.MiniOrdenes;

public record AprobarRechazarJefeCommand(Guid MiniOrdenId, AprobarRechazarMiniOrdenDto Dto, Guid? JefeId, string JefeNombre) : IRequest<MiniOrdenDto>;

public class AprobarRechazarJefeCommandHandler : IRequestHandler<AprobarRechazarJefeCommand, MiniOrdenDto>
{
    private readonly IApplicationDbContext _context;
    public AprobarRechazarJefeCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<MiniOrdenDto> Handle(AprobarRechazarJefeCommand request, CancellationToken cancellationToken)
    {
        var m = await _context.MiniOrdenes.FindAsync([request.MiniOrdenId], cancellationToken)
            ?? throw new NotFoundException("MiniOrden", request.MiniOrdenId);

        if (m.Estado != EstadoMiniOrden.EnRevisionJefe && m.Estado != EstadoMiniOrden.AprobadaJefe)
            throw new InvalidOperationException("La mini-orden no está en revisión del Jefe de Taller.");

        var estadoAnterior = m.Estado;
        // Determinar estado final: aprobado → directo a EnRevisionCliente; rechazado → RechazadaJefe
        var estadoFinal = request.Dto.Aprobado
            ? EstadoMiniOrden.EnRevisionCliente
            : EstadoMiniOrden.RechazadaJefe;

        if (request.Dto.Aprobado)
        {
            m.FechaAprobacionJefe = DateTime.UtcNow;
            if (request.JefeId.HasValue)
                m.JefeTallerId = request.JefeId;
        }
        else m.MotivoRechazo = request.Dto.Observacion;

        m.Estado = estadoFinal;
        m.ActualizadoEn = DateTime.UtcNow;

        _context.MiniOrdenAprobaciones.Add(new MiniOrdenAprobacion
        {
            Id = Guid.NewGuid(), MiniOrdenId = m.Id,
            Nivel = NivelAprobacionMJC.JefeTaller,
            Aprobado = request.Dto.Aprobado,
            AprobadoPorId = request.JefeId,
            AprobadoPorNombre = request.JefeNombre,
            Observacion = request.Dto.Observacion,
            Fecha = DateTime.UtcNow, CreadoEn = DateTime.UtcNow
        });

        _context.MiniOrdenHistoriales.Add(new MiniOrdenHistorial
        {
            Id = Guid.NewGuid(), MiniOrdenId = m.Id,
            EstadoAnterior = estadoAnterior, EstadoNuevo = estadoFinal,
            Observacion = request.Dto.Observacion,
            NivelAprobacion = NivelAprobacionMJC.JefeTaller,
            Fecha = DateTime.UtcNow, CreadoEn = DateTime.UtcNow
        });

        await _context.SaveChangesAsync(cancellationToken);
        return await new GetMiniOrdenByIdQueryHandler(_context).Handle(new GetMiniOrdenByIdQuery(m.Id), cancellationToken);
    }
}
