using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Abstractions;
using Application.Common;
using Application.Extensions;

namespace Application.UseCase.Facturas;

// Para admin/recep: ver todas las solicitudes (con filtro de estado)
public record GetSolicitudesPagoQuery(string? Estado = null, int PageNumber = 1, int PageSize = 20) : IRequest<PagedResult<SolicitudPagoDto>>;

public class GetSolicitudesPagoQueryHandler : IRequestHandler<GetSolicitudesPagoQuery, PagedResult<SolicitudPagoDto>>
{
    private readonly IApplicationDbContext _context;
    public GetSolicitudesPagoQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<PagedResult<SolicitudPagoDto>> Handle(GetSolicitudesPagoQuery request, CancellationToken ct)
    {
        var q = _context.SolicitudesPago
            .Include(s => s.Factura)
            .Include(s => s.Cliente)
            .AsQueryable();

        if (!string.IsNullOrEmpty(request.Estado))
            q = q.Where(s => s.Estado == request.Estado);

        var projected = q.OrderByDescending(s => s.FechaSolicitud)
            .Select(s => new SolicitudPagoDto(
                s.Id, s.FacturaId, s.Factura.NumeroFactura,
                $"{s.Cliente.Nombres} {s.Cliente.Apellidos}",
                s.Monto, s.TipoPago, s.Estado,
                s.Token, s.Referencia, s.Observaciones,
                s.FechaSolicitud, s.FechaConfirmacion, s.ConfirmadoPor
            ));

        return await projected.ToPagedResultAsync(request.PageNumber, request.PageSize, ct);
    }
}

// Para el cliente: ver sus facturas — UsuarioId del JWT → ClienteId en tabla Clientes
public record GetMisFacturasQuery(Guid UsuarioId, int PageNumber = 1, int PageSize = 20) : IRequest<PagedResult<FacturaDto>>;

public class GetMisFacturasQueryHandler : IRequestHandler<GetMisFacturasQuery, PagedResult<FacturaDto>>
{
    private readonly IApplicationDbContext _context;
    public GetMisFacturasQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<PagedResult<FacturaDto>> Handle(GetMisFacturasQuery request, CancellationToken ct)
    {
        // Resolver ClienteId: primero por UsuarioId, luego por email del usuario
        var clienteId = await _context.Clientes
            .Where(c => c.UsuarioId == request.UsuarioId)
            .Select(c => (Guid?)c.Id)
            .FirstOrDefaultAsync(ct);

        if (!clienteId.HasValue)
        {
            // Fallback: buscar por email del usuario
            var userEmail = await _context.Usuarios
                .Where(u => u.Id == request.UsuarioId)
                .Select(u => u.Email)
                .FirstOrDefaultAsync(ct);
            if (!string.IsNullOrEmpty(userEmail))
                clienteId = await _context.Clientes
                    .Where(c => c.Email == userEmail)
                    .Select(c => (Guid?)c.Id)
                    .FirstOrDefaultAsync(ct);
        }

        if (!clienteId.HasValue)
            return new PagedResult<FacturaDto> { Items = [], TotalCount = 0, PageNumber = request.PageNumber, PageSize = request.PageSize };

        var q = _context.Facturas
            .Include(f => f.Cliente)
            .Include(f => f.OrdenServicio)
            .Where(f => f.ClienteId == clienteId.Value)
            .OrderByDescending(f => f.FechaEmision)
            .Select(f => new FacturaDto(
                f.Id, f.NumeroFactura,
                f.OrdenServicioId,
                f.OrdenServicio != null ? f.OrdenServicio.NumeroOrden : null,
                null, // NumerosOrdenes
                f.ClienteId,
                f.Cliente != null ? $"{f.Cliente.Nombres} {f.Cliente.Apellidos}" : null,
                f.Subtotal, f.Impuestos, f.Descuento, f.Total,
                f.Pagada, f.MetodoPago,
                f.FechaEmision, f.CreadoEn
            ));

        return await q.ToPagedResultAsync(request.PageNumber, request.PageSize, ct);
    }
}
