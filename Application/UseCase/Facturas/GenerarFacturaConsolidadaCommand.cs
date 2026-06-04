using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Abstractions;
using Application.Common.Exceptions;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;

namespace Application.UseCase.Facturas;

public record GenerarFacturaConsolidadaCommand(GenerarFacturaConsolidadaDto Dto) : IRequest<FacturaDto>;

public class GenerarFacturaConsolidadaCommandHandler : IRequestHandler<GenerarFacturaConsolidadaCommand, FacturaDto>
{
    private readonly IApplicationDbContext _context;
    public GenerarFacturaConsolidadaCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<FacturaDto> Handle(GenerarFacturaConsolidadaCommand request, CancellationToken cancellationToken)
    {
        // Obtener todas las órdenes finalizadas y sin facturar del cliente
        var ordenes = await _context.OrdenesServicio
            .Include(o => o.DetallesOrdenServicio!)
            .Include(o => o.ManosObra!)
            .Where(o => o.ClienteId == request.Dto.ClienteId
                     && o.Estado == EstadoOrdenEnum.Finalizada
                     && o.FacturaId == null
                     && !o.Eliminado)
            .OrderBy(o => o.FechaIngreso)
            .ToListAsync(cancellationToken);

        if (!ordenes.Any())
            throw new DomainException("No hay órdenes finalizadas sin facturar para este cliente.");

        var cliente = await _context.Clientes
            .FirstOrDefaultAsync(c => c.Id == request.Dto.ClienteId, cancellationToken)
            ?? throw new NotFoundException("Cliente", request.Dto.ClienteId);

        // Calcular subtotal sumando todos los detalles y mano de obra de todas las órdenes
        decimal subtotal = 0;
        foreach (var o in ordenes)
        {
            subtotal += o.DetallesOrdenServicio?.Sum(d => d.Cantidad * d.PrecioUnitario) ?? 0;
            subtotal += o.ManosObra?.Sum(m => m.Costo) ?? 0;
            // Usar Total de la orden si no hay detalles desagregados
            if ((o.DetallesOrdenServicio == null || !o.DetallesOrdenServicio.Any()) &&
                (o.ManosObra == null || !o.ManosObra.Any()) && o.Total.HasValue)
                subtotal += o.Total.Value;
        }

        var descuento      = Math.Min(request.Dto.Descuento, subtotal);
        var baseImponible  = subtotal - descuento;
        var impuestos      = Math.Round(baseImponible * 0.19m, 2);
        var total          = baseImponible + impuestos;

        var factura = new Factura
        {
            NumeroFactura  = $"FAC-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}",
            OrdenServicioId = ordenes.First().Id,  // primera orden como referencia
            ClienteId      = cliente.Id,
            Subtotal       = subtotal,
            Descuento      = descuento,
            Impuestos      = impuestos,
            Total          = total,
            FechaEmision   = DateTime.UtcNow,
            Pagada         = false,
            MetodoPago     = request.Dto.MetodoPago,
            CreadoEn       = DateTime.UtcNow
        };

        _context.Facturas.Add(factura);
        await _context.SaveChangesAsync(cancellationToken); // guardar para obtener el ID

        // Vincular todas las órdenes a esta factura
        foreach (var o in ordenes)
            o.FacturaId = factura.Id;

        await _context.SaveChangesAsync(cancellationToken);

        var numerosOrdenes = ordenes.Select(o => o.NumeroOrden).ToList();
        var clienteNombre  = $"{cliente.Nombres} {cliente.Apellidos}";

        return new FacturaDto(
            factura.Id, factura.NumeroFactura,
            factura.OrdenServicioId,
            ordenes.First().NumeroOrden,
            numerosOrdenes,
            factura.ClienteId, clienteNombre,
            factura.Subtotal, factura.Impuestos, factura.Descuento, factura.Total,
            factura.Pagada, factura.MetodoPago,
            factura.FechaEmision, factura.CreadoEn
        );
    }
}
