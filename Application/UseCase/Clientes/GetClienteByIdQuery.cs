using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Abstractions;
using Application.Common.Exceptions;
using Domain.Enums;

namespace Application.UseCase.Clientes;

public record GetClienteByIdQuery(Guid Id) : IRequest<ClientePerfilDto>;

public class GetClienteByIdQueryHandler : IRequestHandler<GetClienteByIdQuery, ClientePerfilDto>
{
    private readonly IApplicationDbContext _context;

    public GetClienteByIdQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ClientePerfilDto> Handle(GetClienteByIdQuery request, CancellationToken cancellationToken)
    {
        var cliente = await _context.Clientes
            .Include(c => c.TipoDocumento)
            .Include(c => c.Ordenes)
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("Cliente", request.Id);

        var preOrdenesPendientes = await _context.MiniOrdenes
            .Where(m => m.ClienteId == request.Id && m.Estado == EstadoMiniOrden.EnRevisionCliente)
            .ToListAsync(cancellationToken);

        return new ClientePerfilDto(
            cliente.Id,
            cliente.Numero,
            cliente.Nombres,
            cliente.Apellidos,
            cliente.TipoDocumento?.Nombre ?? string.Empty,
            cliente.NumeroDocumento,
            cliente.Email,
            cliente.Telefono,
            cliente.Direccion,
            cliente.CreadoEn,
            cliente.UsuarioId,
            cliente.Ordenes?.Select(o => new ResumenOrdenDto(
                o.Id,
                o.NumeroOrden,
                o.Estado.ToString(),
                o.FechaIngreso,
                o.FechaFin,
                o.Total
            )).OrderByDescending(o => o.FechaIngreso).ToList(),
            preOrdenesPendientes.Select(m => new ResumenMiniOrdenDto(
                m.Id,
                m.NumeroMiniOrden,
                m.Estado.ToString(),
                m.Descripcion,
                m.Total,
                m.CreadoEn
            )).ToList()
        );
    }
}
