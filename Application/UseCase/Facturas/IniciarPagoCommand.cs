using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Abstractions;
using Application.Common.Exceptions;
using Domain.Entities;
using Domain.Exceptions;

namespace Application.UseCase.Facturas;

public record IniciarPagoCommand(Guid UsuarioId, IniciarPagoDto Dto) : IRequest<SolicitudPagoDto>;

public class IniciarPagoCommandHandler : IRequestHandler<IniciarPagoCommand, SolicitudPagoDto>
{
    private readonly IApplicationDbContext _context;
    public IniciarPagoCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<SolicitudPagoDto> Handle(IniciarPagoCommand request, CancellationToken cancellationToken)
    {
        // Resolver ClienteId: por UsuarioId o por email del usuario
        var clienteId = await _context.Clientes
            .Where(c => c.UsuarioId == request.UsuarioId)
            .Select(c => (Guid?)c.Id)
            .FirstOrDefaultAsync(cancellationToken);

        if (!clienteId.HasValue)
        {
            var userEmail = await _context.Usuarios
                .Where(u => u.Id == request.UsuarioId)
                .Select(u => u.Email)
                .FirstOrDefaultAsync(cancellationToken);
            if (!string.IsNullOrEmpty(userEmail))
                clienteId = await _context.Clientes
                    .Where(c => c.Email == userEmail)
                    .Select(c => (Guid?)c.Id)
                    .FirstOrDefaultAsync(cancellationToken);
        }

        var factura = await _context.Facturas
            .Include(f => f.Cliente)
            .FirstOrDefaultAsync(f => f.Id == request.Dto.FacturaId, cancellationToken)
            ?? throw new NotFoundException("Factura", request.Dto.FacturaId);

        // Validar que la factura pertenece al cliente (si el usuario tiene registro de cliente)
        if (clienteId.HasValue && factura.ClienteId != clienteId.Value)
            throw new DomainException("Esta factura no pertenece a tu cuenta.");

        if (factura.Pagada)
            throw new DomainException("Esta factura ya fue pagada.");

        // Verificar que no haya una solicitud pendiente activa
        var solicitudActiva = await _context.SolicitudesPago
            .AnyAsync(s => s.FacturaId == factura.Id && s.Estado == "Pendiente", cancellationToken);
        if (solicitudActiva)
            throw new DomainException("Ya existe una solicitud de pago pendiente para esta factura.");

        var tipos = new[] { "Efectivo", "Tarjeta", "PSE" };
        if (!tipos.Contains(request.Dto.TipoPago))
            throw new DomainException("Tipo de pago inválido. Usa: Efectivo, Tarjeta o PSE.");

        // Generar token para pagos electrónicos
        string? token = null;
        string estado = "Pendiente";
        if (request.Dto.TipoPago is "Tarjeta" or "PSE")
        {
            var prefijo = request.Dto.TipoPago == "Tarjeta" ? "CARD" : "PSE";
            token  = $"{prefijo}-{Guid.NewGuid().ToString()[..8].ToUpper()}-{DateTime.UtcNow:HHmmss}";
            estado = "Procesando"; // Pago electrónico → en procesamiento
        }

        var solicitud = new SolicitudPago
        {
            Id             = Guid.NewGuid(),
            FacturaId      = factura.Id,
            ClienteId      = clienteId ?? factura.ClienteId,
            TipoPago       = request.Dto.TipoPago,
            Estado         = estado,
            Token          = token,
            Referencia     = request.Dto.Referencia,
            Monto          = factura.Total,
            Observaciones  = request.Dto.Observaciones,
            FechaSolicitud = DateTime.UtcNow,
            CreadoEn       = DateTime.UtcNow
        };

        _context.SolicitudesPago.Add(solicitud);

        // Pagos electrónicos → marcar factura como pagada automáticamente con el token
        if (token != null)
        {
            factura.Pagada    = true;
            factura.MetodoPago = $"{request.Dto.TipoPago} ({token})";
            solicitud.Estado  = "Confirmada";
            solicitud.FechaConfirmacion = DateTime.UtcNow;
            solicitud.ConfirmadoPor     = "Sistema (pago electrónico)";
        }

        await _context.SaveChangesAsync(cancellationToken);

        var clienteNombre = factura.Cliente != null
            ? $"{factura.Cliente.Nombres} {factura.Cliente.Apellidos}"
            : "—";

        return new SolicitudPagoDto(
            solicitud.Id, factura.Id, factura.NumeroFactura, clienteNombre,
            solicitud.Monto, solicitud.TipoPago, solicitud.Estado,
            solicitud.Token, solicitud.Referencia, solicitud.Observaciones,
            solicitud.FechaSolicitud, solicitud.FechaConfirmacion, solicitud.ConfirmadoPor
        );
    }
}
