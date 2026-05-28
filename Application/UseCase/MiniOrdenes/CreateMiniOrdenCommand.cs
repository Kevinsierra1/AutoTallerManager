using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Abstractions;
using Application.Common.Exceptions;
using Domain.Entities;
using Domain.Enums;

namespace Application.UseCase.MiniOrdenes;

public record CreateMiniOrdenCommand(CreateMiniOrdenDto Dto, Guid MecanicoId) : IRequest<MiniOrdenDto>;

public class CreateMiniOrdenCommandHandler : IRequestHandler<CreateMiniOrdenCommand, MiniOrdenDto>
{
    private readonly IApplicationDbContext _context;
    public CreateMiniOrdenCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<MiniOrdenDto> Handle(CreateMiniOrdenCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;
        var orden = await _context.OrdenesServicio.FindAsync([dto.OrdenServicioId], cancellationToken)
            ?? throw new NotFoundException("OrdenServicio", dto.OrdenServicioId);

        var contador = await _context.MiniOrdenes.CountAsync(cancellationToken);
        var miniOrden = new MiniOrden
        {
            Id = Guid.NewGuid(),
            NumeroMiniOrden = $"MO-{DateTime.UtcNow:yyyyMMdd}-{contador + 1:D4}",
            OrdenServicioId = dto.OrdenServicioId,
            OrdenAreaId = dto.OrdenAreaId,
            Descripcion = dto.Descripcion,
            Estado = EstadoMiniOrden.Borrador,
            MecanicoId = request.MecanicoId,
            Observaciones = dto.Observaciones,
            CreadoEn = DateTime.UtcNow
        };

        decimal totalMat = 0;
        foreach (var detalle in dto.Detalles)
        {
            var subtotal = detalle.Cantidad * detalle.PrecioUnitario;
            totalMat += subtotal;
            miniOrden.Detalles = miniOrden.Detalles ?? new List<MiniOrdenDetalle>();
            ((List<MiniOrdenDetalle>)miniOrden.Detalles).Add(new MiniOrdenDetalle
            {
                Id = Guid.NewGuid(),
                RepuestoId = detalle.RepuestoId,
                Cantidad = detalle.Cantidad,
                PrecioUnitario = detalle.PrecioUnitario,
                Subtotal = subtotal,
                CreadoEn = DateTime.UtcNow
            });
        }

        decimal totalMO = 0;
        if (dto.ManosObra != null)
        {
            foreach (var mo in dto.ManosObra)
            {
                var total = mo.HorasTrabajo * mo.TarifaHora;
                totalMO += total;
                miniOrden.ManosObra = miniOrden.ManosObra ?? new List<MiniOrdenManoObra>();
                ((List<MiniOrdenManoObra>)miniOrden.ManosObra).Add(new MiniOrdenManoObra
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

        miniOrden.TotalMateriales = totalMat;
        miniOrden.TotalManoObra = totalMO;
        miniOrden.Total = totalMat + totalMO;

        _context.MiniOrdenes.Add(miniOrden);

        _context.MiniOrdenHistoriales.Add(new MiniOrdenHistorial
        {
            Id = Guid.NewGuid(),
            MiniOrdenId = miniOrden.Id,
            EstadoAnterior = EstadoMiniOrden.Borrador,
            EstadoNuevo = EstadoMiniOrden.Borrador,
            Observacion = "Mini-orden creada por mecánico",
            NivelAprobacion = NivelAprobacionMJC.Mecanico,
            Fecha = DateTime.UtcNow,
            CreadoEn = DateTime.UtcNow
        });

        await _context.SaveChangesAsync(cancellationToken);

        return await new GetMiniOrdenByIdQueryHandler(_context)
            .Handle(new GetMiniOrdenByIdQuery(miniOrden.Id), cancellationToken);
    }
}
