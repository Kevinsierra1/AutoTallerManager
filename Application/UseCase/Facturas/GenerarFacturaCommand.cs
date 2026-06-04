using MediatR;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Application.Abstractions;
using Application.Common.Exceptions;
using Domain.Entities;
using Domain.Enums;

namespace Application.UseCase.Facturas;

public record GenerarFacturaCommand(GenerarFacturaDto Dto) : IRequest<FacturaDto>;

public class GenerarFacturaCommandHandler : IRequestHandler<GenerarFacturaCommand, FacturaDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GenerarFacturaCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<FacturaDto> Handle(GenerarFacturaCommand request, CancellationToken cancellationToken)
    {
        var orden = await _context.OrdenesServicio
            .Include(o => o.DetallesOrdenServicio).ThenInclude(d => d.Repuesto)
            .Include(o => o.ManosObra)
            .FirstOrDefaultAsync(o => o.Id == request.Dto.OrdenServicioId, cancellationToken)
            ?? throw new NotFoundException("OrdenServicio", request.Dto.OrdenServicioId);

        if (orden.Estado != EstadoOrdenEnum.Finalizada)
            throw new Domain.Exceptions.DomainException("Solo se puede facturar órdenes finalizadas.");
        if (orden.FacturaId.HasValue)
            throw new Domain.Exceptions.DomainException("Esta orden ya fue incluida en una factura.");

        var subtotalRepuestos = orden.DetallesOrdenServicio?.Sum(d => d.Cantidad * d.PrecioUnitario) ?? 0;
        var subtotalManoObra  = orden.ManosObra?.Sum(m => m.Costo) ?? 0;
        var subtotal  = subtotalRepuestos + subtotalManoObra;
        // Si la orden tiene un Total ya calculado y no hay detalles cargados, usar ese
        if (subtotal == 0 && orden.Total.HasValue)
            subtotal = orden.Total.Value;
        var descuento = Math.Min(request.Dto.Descuento, subtotal); // No puede exceder el subtotal
        var baseImponible = subtotal - descuento;
        var impuestos = Math.Round(baseImponible * 0.19m, 2); // 19% IVA
        var total     = baseImponible + impuestos;

        // Verificar que no se haya facturado ya
        var yaFacturada = await _context.Facturas
            .AnyAsync(f => f.OrdenServicioId == orden.Id, cancellationToken);
        if (yaFacturada)
            throw new Domain.Exceptions.DomainException("Esta orden ya tiene una factura generada.");

        var factura = new Factura
        {
            NumeroFactura = $"FAC-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}",
            OrdenServicioId = orden.Id,
            ClienteId = orden.ClienteId,
            Subtotal = subtotal,
            Descuento = descuento,
            Impuestos = impuestos,
            Total = total,
            FechaEmision = DateTime.UtcNow,
            Pagada = false,
            MetodoPago = request.Dto.MetodoPago
        };

        _context.Facturas.Add(factura);
        await _context.SaveChangesAsync(cancellationToken);

        // Recargar con navegaciones para el mapper
        // Vincular la orden a la factura
        orden.FacturaId = factura.Id;
        await _context.SaveChangesAsync(cancellationToken);

        var guardada = await _context.Facturas
            .Include(f => f.Cliente)
            .Include(f => f.OrdenServicio)
            .Include(f => f.Ordenes)
            .FirstAsync(f => f.Id == factura.Id, cancellationToken);
        return _mapper.Map<FacturaDto>(guardada);
    }
}
