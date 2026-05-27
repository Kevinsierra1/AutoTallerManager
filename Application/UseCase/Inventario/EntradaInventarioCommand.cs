using MediatR;
using Application.Abstractions;
using Application.Common.Exceptions;
using Domain.Entities;
using Domain.Enums;

namespace Application.UseCase.Inventario;

public record EntradaInventarioCommand(EntradaInventarioDto Dto) : IRequest;

public class EntradaInventarioCommandHandler : IRequestHandler<EntradaInventarioCommand>
{
    private readonly IApplicationDbContext _context;
    public EntradaInventarioCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(EntradaInventarioCommand request, CancellationToken cancellationToken)
    {
        var repuesto = await _context.Repuestos.FindAsync([request.Dto.RepuestoId], cancellationToken)
            ?? throw new NotFoundException("Repuesto", request.Dto.RepuestoId);

        var anterior = repuesto.StockActual;
        repuesto.StockActual += request.Dto.Cantidad;

        var movimiento = new MovimientoInventario
        {
            RepuestoId = repuesto.Id,
            Tipo = TipoMovimientoInventario.Entrada,
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
