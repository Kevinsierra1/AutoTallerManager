using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Abstractions;
using Application.Common.Exceptions;
using Domain.Entities;
using Domain.Enums;

namespace Application.UseCase.MiniOrdenes;

public record CreateMiniOrdenCommand(CreatePresupuestoDto Dto, Guid? MecanicoId) : IRequest<MiniOrdenDto>;

public class CreateMiniOrdenCommandHandler : IRequestHandler<CreateMiniOrdenCommand, MiniOrdenDto>
{
    private readonly IApplicationDbContext _context;
    public CreateMiniOrdenCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<MiniOrdenDto> Handle(CreateMiniOrdenCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        _ = await _context.Clientes.FindAsync([dto.ClienteId], cancellationToken)
            ?? throw new NotFoundException("Cliente", dto.ClienteId);

        _ = await _context.Vehiculos.FindAsync([dto.VehiculoId], cancellationToken)
            ?? throw new NotFoundException("Vehículo", dto.VehiculoId);

        // Validar que el vehículo pertenece al cliente seleccionado
        var perteneceAlCliente = await _context.VehiculoPropietarios
            .AnyAsync(vp => vp.VehiculoId == dto.VehiculoId
                         && vp.ClienteId  == dto.ClienteId
                         && vp.Activo, cancellationToken);
        if (!perteneceAlCliente)
            throw new Domain.Exceptions.DomainException("El vehículo no está registrado a nombre del cliente seleccionado.");

        var contador = await _context.MiniOrdenes.CountAsync(cancellationToken);
        var presupuesto = new MiniOrden
        {
            Id = Guid.NewGuid(),
            NumeroMiniOrden = $"PRES-{DateTime.UtcNow:yyyyMMdd}-{contador + 1:D4}",
            ClienteId = dto.ClienteId,
            VehiculoId = dto.VehiculoId,
            OrdenServicioId = null,   // se asigna cuando el cliente apruebe
            Descripcion = dto.Descripcion,
            Estado = EstadoMiniOrden.Borrador,
            MecanicoId = request.MecanicoId,
            Observaciones = dto.Observaciones,
            CreadoEn = DateTime.UtcNow
        };

        decimal totalMat = 0;
        if (dto.Detalles?.Count > 0)
        {
            presupuesto.Detalles = new List<MiniOrdenDetalle>();
            foreach (var d in dto.Detalles)
            {
                var subtotal = d.Cantidad * d.PrecioUnitario;
                totalMat += subtotal;
                ((List<MiniOrdenDetalle>)presupuesto.Detalles).Add(new MiniOrdenDetalle
                {
                    Id = Guid.NewGuid(),
                    RepuestoId = d.RepuestoId,
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario,
                    Subtotal = subtotal,
                    CreadoEn = DateTime.UtcNow
                });
            }
        }

        decimal totalMO = 0;
        if (dto.ManosObra?.Count > 0)
        {
            presupuesto.ManosObra = new List<MiniOrdenManoObra>();
            foreach (var mo in dto.ManosObra)
            {
                var total = mo.HorasTrabajo * mo.TarifaHora;
                totalMO += total;
                ((List<MiniOrdenManoObra>)presupuesto.ManosObra).Add(new MiniOrdenManoObra
                {
                    Id = Guid.NewGuid(),
                    Descripcion = mo.Descripcion,
                    HorasTrabajo = mo.HorasTrabajo,
                    TarifaHora = mo.TarifaHora,
                    Total = total,
                    TecnicoId = mo.TecnicoId,
                    CreadoEn = DateTime.UtcNow
                });
            }
        }

        presupuesto.TotalMateriales = totalMat;
        presupuesto.TotalManoObra = totalMO;
        presupuesto.Total = totalMat + totalMO;

        _context.MiniOrdenes.Add(presupuesto);

        _context.MiniOrdenHistoriales.Add(new MiniOrdenHistorial
        {
            Id = Guid.NewGuid(),
            MiniOrdenId = presupuesto.Id,
            EstadoAnterior = EstadoMiniOrden.Borrador,
            EstadoNuevo = EstadoMiniOrden.Borrador,
            Observacion = "Presupuesto creado por mecánico",
            NivelAprobacion = NivelAprobacionMJC.Mecanico,
            Fecha = DateTime.UtcNow,
            CreadoEn = DateTime.UtcNow
        });

        await _context.SaveChangesAsync(cancellationToken);

        return await new GetMiniOrdenByIdQueryHandler(_context)
            .Handle(new GetMiniOrdenByIdQuery(presupuesto.Id), cancellationToken);
    }
}
