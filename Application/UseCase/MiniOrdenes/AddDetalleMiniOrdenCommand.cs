using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Abstractions;
using Application.Common.Exceptions;
using Domain.Entities;

namespace Application.UseCase.MiniOrdenes;

public record AddDetalleMiniOrdenCommand(Guid MiniOrdenId, CreateMiniOrdenDetalleDto Dto) : IRequest<MiniOrdenDetalleDto>;

public class AddDetalleMiniOrdenCommandHandler : IRequestHandler<AddDetalleMiniOrdenCommand, MiniOrdenDetalleDto>
{
    private readonly IApplicationDbContext _context;
    public AddDetalleMiniOrdenCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<MiniOrdenDetalleDto> Handle(AddDetalleMiniOrdenCommand request, CancellationToken cancellationToken)
    {
        var mini = await _context.MiniOrdenes
            .Include(m => m.Detalles)
            .FirstOrDefaultAsync(m => m.Id == request.MiniOrdenId, cancellationToken)
            ?? throw new NotFoundException("Presupuesto", request.MiniOrdenId);

        var repuesto = await _context.Repuestos
            .FirstOrDefaultAsync(r => r.Id == request.Dto.RepuestoId, cancellationToken)
            ?? throw new NotFoundException("Repuesto", request.Dto.RepuestoId);

        var subtotal = request.Dto.Cantidad * request.Dto.PrecioUnitario;
        var detalle = new MiniOrdenDetalle
        {
            Id = Guid.NewGuid(),
            MiniOrdenId = mini.Id,
            RepuestoId = request.Dto.RepuestoId,
            Cantidad = request.Dto.Cantidad,
            PrecioUnitario = request.Dto.PrecioUnitario,
            Subtotal = subtotal,
            CreadoEn = DateTime.UtcNow
        };

        // Capturar la suma ANTES de Add para evitar el doble conteo por EF relationship fixup
        var existingMat = (mini.Detalles ?? []).Sum(d => d.Subtotal);
        _context.MiniOrdenDetalles.Add(detalle);

        var totalMat = existingMat + subtotal;
        mini.TotalMateriales = totalMat;
        mini.Total = totalMat + mini.TotalManoObra;

        await _context.SaveChangesAsync(cancellationToken);

        return new MiniOrdenDetalleDto(detalle.Id, detalle.RepuestoId, repuesto.Nombre,
            repuesto.Codigo, detalle.Cantidad, detalle.PrecioUnitario, detalle.Subtotal);
    }
}
