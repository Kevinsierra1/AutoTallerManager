using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Abstractions;
using Application.Common.Exceptions;
using Domain.Entities;

namespace Application.UseCase.Ordenes;

public record AddDetalleOrdenCommand(Guid OrdenId, CreateDetalleOrdenDto Dto) : IRequest<DetalleOrdenDto>;

public class AddDetalleOrdenCommandHandler : IRequestHandler<AddDetalleOrdenCommand, DetalleOrdenDto>
{
    private readonly IApplicationDbContext _context;

    public AddDetalleOrdenCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<DetalleOrdenDto> Handle(AddDetalleOrdenCommand request, CancellationToken cancellationToken)
    {
        var orden = await _context.OrdenesServicio
            .Include(o => o.DetallesOrdenServicio)
            .FirstOrDefaultAsync(o => o.Id == request.OrdenId, cancellationToken)
            ?? throw new NotFoundException("OrdenServicio", request.OrdenId);

        var repuesto = await _context.Repuestos
            .FirstOrDefaultAsync(r => r.Id == request.Dto.RepuestoId, cancellationToken)
            ?? throw new NotFoundException("Repuesto", request.Dto.RepuestoId);

        var detalle = new DetalleOrdenServicio
        {
            Id = Guid.NewGuid(),
            OrdenServicioId = orden.Id,
            RepuestoId = request.Dto.RepuestoId,
            Cantidad = request.Dto.Cantidad,
            PrecioUnitario = request.Dto.PrecioUnitario,
            CreadoEn = DateTime.UtcNow
        };

        // Capturar suma ANTES de Add para evitar doble conteo por EF relationship fixup
        var existingDetalles = (orden.DetallesOrdenServicio ?? [])
            .Sum(d => d.Cantidad * d.PrecioUnitario);
        _context.DetallesOrdenServicio.Add(detalle);

        var subtotalDetalles = existingDetalles + detalle.Cantidad * detalle.PrecioUnitario;

        var subtotalManosObra = await _context.ManosObra
            .Where(m => m.OrdenServicioId == orden.Id)
            .SumAsync(m => m.Costo, cancellationToken);

        orden.Total = subtotalDetalles + subtotalManosObra;

        await _context.SaveChangesAsync(cancellationToken);

        return new DetalleOrdenDto(detalle.Id, detalle.RepuestoId, repuesto.Nombre,
            detalle.Cantidad, detalle.PrecioUnitario, detalle.Subtotal);
    }
}
