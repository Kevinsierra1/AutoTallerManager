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

            // ── Buscar una OS existente del mismo cliente+vehículo aprobada hoy ──
            // para consolidar todos los servicios aprobados en UNA sola orden.
            var hoy = DateTime.UtcNow.Date;
            var osIdExistente = await _context.MiniOrdenes
                .Where(mo => mo.ClienteId  == m.ClienteId
                          && mo.VehiculoId == m.VehiculoId
                          && mo.Id         != m.Id
                          && mo.OrdenServicioId.HasValue
                          && mo.Estado == EstadoMiniOrden.EnProceso
                          && mo.FechaAprobacionCliente.HasValue
                          && mo.FechaAprobacionCliente.Value >= hoy)
                .Select(mo => mo.OrdenServicioId)
                .FirstOrDefaultAsync(cancellationToken);

            Guid ordenId;

            if (osIdExistente.HasValue)
            {
                // Reusar la OS existente — agregar descripción de este servicio
                ordenId = osIdExistente.Value;
                var osExistente = await _context.OrdenesServicio
                    .FirstOrDefaultAsync(os => os.Id == ordenId, cancellationToken);
                if (osExistente != null)
                    osExistente.Descripcion += $"\n• {m.Descripcion}";
            }
            else
            {
                // Crear una nueva OS que consolidará todos los servicios
                var contador = await _context.OrdenesServicio.CountAsync(cancellationToken);
                var nuevaOrden = new OrdenServicio
                {
                    Id          = Guid.NewGuid(),
                    NumeroOrden = $"OS-{DateTime.UtcNow:yyyyMMdd}-{contador + 1:D4}",
                    ClienteId   = m.ClienteId,
                    VehiculoId  = m.VehiculoId,
                    MecanicoId  = m.MecanicoId,
                    Estado      = EstadoOrdenEnum.Aprobada,
                    Descripcion = m.Descripcion,
                    FechaIngreso = DateTime.UtcNow,
                    CreadoEn    = DateTime.UtcNow
                };
                _context.OrdenesServicio.Add(nuevaOrden);
                ordenId = nuevaOrden.Id;

                _context.HistorialEstadosOrden.Add(new HistorialEstadoOrden
                {
                    Id              = Guid.NewGuid(),
                    OrdenServicioId = ordenId,
                    Estado          = EstadoOrdenEnum.Aprobada,
                    Observaciones   = $"Orden generada desde presupuesto {m.NumeroMiniOrden} aprobado por {request.ClienteNombre}",
                    FechaHora       = DateTime.UtcNow,
                    CreadoEn        = DateTime.UtcNow
                });
            }

            m.OrdenServicioId = ordenId;
            m.Estado     = EstadoMiniOrden.EnProceso;
            m.FechaInicio = DateTime.UtcNow;
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
