using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Abstractions;
using Application.Common.Exceptions;
using Domain.Exceptions;

namespace Application.UseCase.Facturas;

public record ConfirmarPagoEfectivoCommand(Guid SolicitudId, string ConfirmadoPor, string? Observaciones) : IRequest<SolicitudPagoDto>;

public class ConfirmarPagoEfectivoCommandHandler : IRequestHandler<ConfirmarPagoEfectivoCommand, SolicitudPagoDto>
{
    private readonly IApplicationDbContext _context;
    public ConfirmarPagoEfectivoCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<SolicitudPagoDto> Handle(ConfirmarPagoEfectivoCommand request, CancellationToken cancellationToken)
    {
        var solicitud = await _context.SolicitudesPago
            .Include(s => s.Factura).ThenInclude(f => f.Cliente)
            .FirstOrDefaultAsync(s => s.Id == request.SolicitudId, cancellationToken)
            ?? throw new NotFoundException("SolicitudPago", request.SolicitudId);

        if (solicitud.Estado != "Pendiente")
            throw new DomainException($"La solicitud ya fue procesada (estado: {solicitud.Estado}).");

        if (solicitud.TipoPago != "Efectivo")
            throw new DomainException("Solo se pueden confirmar pagos en efectivo manualmente.");

        solicitud.Estado             = "Confirmada";
        solicitud.FechaConfirmacion  = DateTime.UtcNow;
        solicitud.ConfirmadoPor      = request.ConfirmadoPor;
        solicitud.Observaciones      = request.Observaciones ?? solicitud.Observaciones;
        solicitud.ActualizadoEn      = DateTime.UtcNow;

        solicitud.Factura.Pagada    = true;
        solicitud.Factura.MetodoPago = "Efectivo";
        solicitud.Factura.ActualizadoEn = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        var clienteNombre = solicitud.Factura.Cliente != null
            ? $"{solicitud.Factura.Cliente.Nombres} {solicitud.Factura.Cliente.Apellidos}"
            : "—";

        return new SolicitudPagoDto(
            solicitud.Id, solicitud.FacturaId, solicitud.Factura.NumeroFactura, clienteNombre,
            solicitud.Monto, solicitud.TipoPago, solicitud.Estado,
            solicitud.Token, solicitud.Referencia, solicitud.Observaciones,
            solicitud.FechaSolicitud, solicitud.FechaConfirmacion, solicitud.ConfirmadoPor
        );
    }
}
