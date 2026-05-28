using MediatR;
using Application.Abstractions;
using Application.Common.Exceptions;
using Domain.Entities;
using Domain.Enums;

namespace Application.UseCase.MiniOrdenes;

public record AprobarRechazarClienteCommand(Guid MiniOrdenId, AprobarRechazarMiniOrdenDto Dto, string ClienteNombre) : IRequest<MiniOrdenDto>;

public class AprobarRechazarClienteCommandHandler : IRequestHandler<AprobarRechazarClienteCommand, MiniOrdenDto>
{
    private readonly IApplicationDbContext _context;
    public AprobarRechazarClienteCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<MiniOrdenDto> Handle(AprobarRechazarClienteCommand request, CancellationToken cancellationToken)
    {
        var m = await _context.MiniOrdenes.FindAsync([request.MiniOrdenId], cancellationToken)
            ?? throw new NotFoundException("MiniOrden", request.MiniOrdenId);

        if (m.Estado != EstadoMiniOrden.EnRevisionCliente)
            throw new InvalidOperationException("La mini-orden no está pendiente de aprobación del Cliente.");

        var estadoAnterior = m.Estado;
        m.Estado = request.Dto.Aprobado ? EstadoMiniOrden.AprobadaCliente : EstadoMiniOrden.RechazadaCliente;
        if (request.Dto.Aprobado) m.FechaAprobacionCliente = DateTime.UtcNow;
        else m.MotivoRechazo = request.Dto.Observacion;
        m.ActualizadoEn = DateTime.UtcNow;

        _context.MiniOrdenAprobaciones.Add(new MiniOrdenAprobacion
        {
            Id = Guid.NewGuid(), MiniOrdenId = m.Id,
            Nivel = NivelAprobacionMJC.Cliente,
            Aprobado = request.Dto.Aprobado,
            AprobadoPorNombre = request.ClienteNombre,
            Observacion = request.Dto.Observacion,
            Fecha = DateTime.UtcNow, CreadoEn = DateTime.UtcNow
        });

        _context.MiniOrdenHistoriales.Add(new MiniOrdenHistorial
        {
            Id = Guid.NewGuid(), MiniOrdenId = m.Id,
            EstadoAnterior = estadoAnterior, EstadoNuevo = m.Estado,
            Observacion = request.Dto.Observacion,
            NivelAprobacion = NivelAprobacionMJC.Cliente,
            Fecha = DateTime.UtcNow, CreadoEn = DateTime.UtcNow
        });

        if (request.Dto.Aprobado)
            m.Estado = EstadoMiniOrden.EnProceso;

        await _context.SaveChangesAsync(cancellationToken);
        return await new GetMiniOrdenByIdQueryHandler(_context).Handle(new GetMiniOrdenByIdQuery(m.Id), cancellationToken);
    }
}
