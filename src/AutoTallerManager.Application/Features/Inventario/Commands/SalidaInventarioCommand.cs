using MediatR;
using AutoTallerManager.Application.Common.Interfaces;
using AutoTallerManager.Application.Common.Exceptions;
using AutoTallerManager.Application.Features.Inventario.DTOs;
using AutoTallerManager.Domain.Entities;
using AutoTallerManager.Domain.Enums;
using AutoTallerManager.Domain.Exceptions;

namespace AutoTallerManager.Application.Features.Inventario.Commands;

public record SalidaInventarioCommand(SalidaInventarioDto Dto) : IRequest;

public class SalidaInventarioCommandHandler : IRequestHandler<SalidaInventarioCommand>
{
    private readonly IApplicationDbContext _context;
    public SalidaInventarioCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(SalidaInventarioCommand request, CancellationToken cancellationToken)
    {
        var repuesto = await _context.Repuestos.FindAsync([request.Dto.RepuestoId], cancellationToken)
            ?? throw new NotFoundException("Repuesto", request.Dto.RepuestoId);

        if (repuesto.StockActual < request.Dto.Cantidad)
            throw new StockInsuficienteException(repuesto.Nombre, repuesto.StockActual, request.Dto.Cantidad);

        var anterior = repuesto.StockActual;
        repuesto.StockActual -= request.Dto.Cantidad;

        var movimiento = new MovimientoInventario
        {
            RepuestoId = repuesto.Id,
            Tipo = TipoMovimientoInventario.Salida,
            Cantidad = request.Dto.Cantidad,
            CantidadAnterior = anterior,
            CantidadNueva = repuesto.StockActual,
            Motivo = request.Dto.Motivo,
            FechaMovimiento = DateTime.UtcNow
        };

        _context.MovimientosInventario.Add(movimiento);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
