using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Abstractions;
using Application.Common.Exceptions;
using Domain.Entities;
using Domain.Exceptions;

namespace Application.UseCase.Facturas;

public record RegistrarPagoDto(
    Guid MetodoPagoId,
    decimal Monto,
    string? Referencia
);

public record RegistrarPagoCommand(Guid FacturaId, RegistrarPagoDto Dto) : IRequest<FacturaDto>;

public class RegistrarPagoCommandHandler : IRequestHandler<RegistrarPagoCommand, FacturaDto>
{
    private readonly IApplicationDbContext _context;
    public RegistrarPagoCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<FacturaDto> Handle(RegistrarPagoCommand request, CancellationToken cancellationToken)
    {
        var factura = await _context.Facturas
            .Include(f => f.Cliente)
            .Include(f => f.OrdenServicio)
            .FirstOrDefaultAsync(f => f.Id == request.FacturaId, cancellationToken)
            ?? throw new NotFoundException("Factura", request.FacturaId);

        if (factura.Pagada)
            throw new DomainException("Esta factura ya fue pagada.");

        var metodoPago = await _context.MetodosPago
            .FirstOrDefaultAsync(m => m.Id == request.Dto.MetodoPagoId && m.Activo, cancellationToken)
            ?? throw new NotFoundException("MetodoPago", request.Dto.MetodoPagoId);

        if (request.Dto.Monto <= 0)
            throw new DomainException("El monto del pago debe ser mayor a cero.");

        if (request.Dto.Monto < factura.Total)
            throw new DomainException($"El monto ${request.Dto.Monto:N0} es menor que el total de la factura ${factura.Total:N0}.");

        _context.Pagos.Add(new Pago
        {
            Id           = Guid.NewGuid(),
            FacturaId    = factura.Id,
            MetodoPagoId = metodoPago.Id,
            Monto        = request.Dto.Monto,
            FechaPago    = DateTime.UtcNow,
            Referencia   = request.Dto.Referencia,
            CreadoEn     = DateTime.UtcNow
        });

        factura.Pagada    = true;
        factura.MetodoPago = metodoPago.Nombre;
        factura.ActualizadoEn = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return new FacturaDto(
            factura.Id, factura.NumeroFactura,
            factura.OrdenServicioId,
            factura.OrdenServicio?.NumeroOrden,
            null, // NumerosOrdenes — no cargado en este contexto
            factura.ClienteId,
            factura.Cliente != null ? $"{factura.Cliente.Nombres} {factura.Cliente.Apellidos}" : null,
            factura.Subtotal, factura.Impuestos, factura.Descuento, factura.Total,
            factura.Pagada, factura.MetodoPago,
            factura.FechaEmision, factura.CreadoEn
        );
    }
}
