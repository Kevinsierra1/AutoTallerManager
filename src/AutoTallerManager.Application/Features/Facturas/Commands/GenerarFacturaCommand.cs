using MediatR;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using AutoTallerManager.Application.Features.Facturas.DTOs;
using AutoTallerManager.Application.Common.Interfaces;
using AutoTallerManager.Application.Common.Exceptions;
using AutoTallerManager.Domain.Entities;
using AutoTallerManager.Domain.Enums;

namespace AutoTallerManager.Application.Features.Facturas.Commands;

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

        var subtotalRepuestos = orden.DetallesOrdenServicio?.Sum(d => d.Cantidad * d.PrecioUnitario) ?? 0;
        var subtotalManoObra = orden.ManosObra?.Sum(m => m.Costo) ?? 0;
        var subtotal = subtotalRepuestos + subtotalManoObra;
        var descuento = request.Dto.Descuento;
        var impuestos = (subtotal - descuento) * 0.19m; // 19% IVA
        var total = subtotal - descuento + impuestos;

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
            Pagada = false
        };

        _context.Facturas.Add(factura);
        await _context.SaveChangesAsync(cancellationToken);
        return _mapper.Map<FacturaDto>(factura);
    }
}
