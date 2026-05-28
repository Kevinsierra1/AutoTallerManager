using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Abstractions;
using Application.Common.Exceptions;
using Domain.Entities;
using Domain.Enums;

namespace Application.UseCase.MiniOrdenes;

public record AprobarRechazarClienteCommand(
    Guid MiniOrdenId,
    AprobarRechazarMiniOrdenDto Dto,
    string ClienteNombre
) : IRequest<MiniOrdenDto>;

public class AprobarRechazarClienteCommandHandler : IRequestHandler<AprobarRechazarClienteCommand, MiniOrdenDto>
{
    private readonly IApplicationDbContext _context;
    public AprobarRechazarClienteCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<MiniOrdenDto> Handle(AprobarRechazarClienteCommand request, CancellationToken cancellationToken)
    {
        var m = await _context.MiniOrdenes
            .Include(x => x.Cliente)
            .Include(x => x.Vehiculo)
            .FirstOrDefaultAsync(x => x.Id == request.MiniOrdenId, cancellationToken)
            ?? throw new NotFoundException("Presupuesto", request.MiniOrdenId);

        if (m.Estado != EstadoMiniOrden.EnRevisionCliente)
            throw new InvalidOperationException("El presupuesto no está pendiente de aprobación del Cliente.");

        var estadoAnterior = m.Estado;

        if (request.Dto.Aprobado)
        {
            m.Estado = EstadoMiniOrden.AprobadaCliente;
            m.FechaAprobacionCliente = DateTime.UtcNow;

            // ── Crear la Orden de Servicio automáticamente ─────────────────
            var contador = await _context.OrdenesServicio.CountAsync(cancellationToken);
            var nuevaOrden = new OrdenServicio
            {
                Id = Guid.NewGuid(),
                NumeroOrden = $"OS-{DateTime.UtcNow:yyyyMMdd}-{contador + 1:D4}",
                ClienteId = m.ClienteId,
                VehiculoId = m.VehiculoId,
                MecanicoId = m.MecanicoId,
                Estado = EstadoOrdenEnum.Aprobada,
                Descripcion = m.Descripcion,
                FechaIngreso = DateTime.UtcNow,
                CreadoEn = DateTime.UtcNow
            };

            _context.OrdenesServicio.Add(nuevaOrden);

            // Vinculamos el presupuesto a la OS recién creada
            m.OrdenServicioId = nuevaOrden.Id;
            m.Estado = EstadoMiniOrden.EnProceso;
            m.FechaInicio = DateTime.UtcNow;

            _context.HistorialEstadosOrden.Add(new HistorialEstadoOrden
            {
                Id = Guid.NewGuid(),
                OrdenServicioId = nuevaOrden.Id,
                Estado = EstadoOrdenEnum.Aprobada,
                Observaciones = $"Orden generada desde presupuesto {m.NumeroMiniOrden} aprobado por {request.ClienteNombre}",
                FechaHora = DateTime.UtcNow,
                CreadoEn = DateTime.UtcNow
            });
        }
        else
        {
            m.Estado = EstadoMiniOrden.RechazadaCliente;
            m.MotivoRechazo = request.Dto.Observacion;
        }

        m.ActualizadoEn = DateTime.UtcNow;

        _context.MiniOrdenAprobaciones.Add(new MiniOrdenAprobacion
        {
            Id = Guid.NewGuid(),
            MiniOrdenId = m.Id,
            Nivel = NivelAprobacionMJC.Cliente,
            Aprobado = request.Dto.Aprobado,
            AprobadoPorNombre = request.ClienteNombre,
            Observacion = request.Dto.Observacion,
            Fecha = DateTime.UtcNow,
            CreadoEn = DateTime.UtcNow
        });

        _context.MiniOrdenHistoriales.Add(new MiniOrdenHistorial
        {
            Id = Guid.NewGuid(),
            MiniOrdenId = m.Id,
            EstadoAnterior = estadoAnterior,
            EstadoNuevo = m.Estado,
            Observacion = request.Dto.Aprobado
                ? $"Cliente aprobó. Orden de Servicio generada automáticamente."
                : request.Dto.Observacion,
            NivelAprobacion = NivelAprobacionMJC.Cliente,
            Fecha = DateTime.UtcNow,
            CreadoEn = DateTime.UtcNow
        });

        await _context.SaveChangesAsync(cancellationToken);

        return await new GetMiniOrdenByIdQueryHandler(_context)
            .Handle(new GetMiniOrdenByIdQuery(m.Id), cancellationToken);
    }
}
