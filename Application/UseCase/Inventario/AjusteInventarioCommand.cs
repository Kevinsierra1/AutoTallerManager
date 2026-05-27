using MediatR;
using Application.Abstractions;
using Application.Common.Exceptions;
using Domain.Entities;
using Domain.Enums;

namespace Application.UseCase.Inventario;

public record AjusteInventarioCommand(Guid RepuestoId, int NuevaCantidad, string Motivo) : IRequest;

public class AjusteInventarioCommandHandler : IRequestHandler<AjusteInventarioCommand>
{
    private readonly IApplicationDbContext _context;
    public AjusteInventarioCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(AjusteInventarioCommand request, CancellationToken cancellationToken)
    {
        var repuesto = await _context.Repuestos.FindAsync([request.RepuestoId], cancellationToken)
            ?? throw new NotFoundException("Repuesto", request.RepuestoId);
        var diferencia = request.NuevaCantidad - repuesto.StockActual;
        repuesto.StockActual = request.NuevaCantidad;
        var movimiento = new MovimientoInventario
        {
            RepuestoId = request.RepuestoId,
            Tipo = diferencia >= 0 ? TipoMovimientoInventario.Entrada : TipoMovimientoInventario.Salida,
            Cantidad = Math.Abs(diferencia),
            CantidadAnterior = repuesto.StockActual + diferencia,
            CantidadNueva = request.NuevaCantidad,
            Motivo = request.Motivo,
            FechaMovimiento = DateTime.UtcNow
        };
        _context.MovimientosInventario.Add(movimiento);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
