using MediatR;
using Application.Abstractions;
using Application.Common.Exceptions;
using Domain.Entities;
using Domain.Enums;

namespace Application.UseCase.MiniOrdenes;

public record AprobarRechazarJefeCommand(Guid MiniOrdenId, AprobarRechazarMiniOrdenDto Dto, Guid JefeId, string JefeNombre) : IRequest<MiniOrdenDto>;

public class AprobarRechazarJefeCommandHandler : IRequestHandler<AprobarRechazarJefeCommand, MiniOrdenDto>
{
    private readonly IApplicationDbContext _context;
    public AprobarRechazarJefeCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<MiniOrdenDto> Handle(AprobarRechazarJefeCommand request, CancellationToken cancellationToken)
    {
        var m = await _context.MiniOrdenes.FindAsync([request.MiniOrdenId], cancellationToken)
            ?? throw new NotFoundException("MiniOrden", request.MiniOrdenId);

        if (m.Estado != EstadoMiniOrden.EnRevisionJefe)
            throw new InvalidOperationException("La mini-orden no está pendiente de aprobación del Jefe de Taller.");

        var estadoAnterior = m.Estado;
        m.Estado = request.Dto.Aprobado ? EstadoMiniOrden.AprobadaJefe : EstadoMiniOrden.RechazadaJefe;
        if (request.Dto.Aprobado) { m.FechaAprobacionJefe = DateTime.UtcNow; m.JefeTallerId = request.JefeId; }
        else m.MotivoRechazo = request.Dto.Observacion;
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
            EstadoAnterior = estadoAnterior, EstadoNuevo = m.Estado,
            Observacion = request.Dto.Observacion,
            NivelAprobacion = NivelAprobacionMJC.JefeTaller,
            Fecha = DateTime.UtcNow, CreadoEn = DateTime.UtcNow
        });

        if (request.Dto.Aprobado)
            m.Estado = EstadoMiniOrden.EnRevisionCliente;

        await _context.SaveChangesAsync(cancellationToken);
        return await new GetMiniOrdenByIdQueryHandler(_context).Handle(new GetMiniOrdenByIdQuery(m.Id), cancellationToken);
    }
}
