using MediatR;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Application.Abstractions;
using Application.Common.Exceptions;

namespace Application.UseCase.Ordenes;

public record GetOrdenByIdQuery(Guid Id) : IRequest<OrdenServicioDto>;

public class GetOrdenByIdQueryHandler : IRequestHandler<GetOrdenByIdQuery, OrdenServicioDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetOrdenByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<OrdenServicioDto> Handle(GetOrdenByIdQuery request, CancellationToken cancellationToken)
    {
        var orden = await _context.OrdenesServicio
            .Include(o => o.Cliente)
            .Include(o => o.Vehiculo)
            .Include(o => o.Mecanico)
            .Include(o => o.DetallesOrdenServicio!)
                .ThenInclude(d => d.Repuesto)
            .Include(o => o.ManosObra!)
                .ThenInclude(m => m.Empleado)
            .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("OrdenServicio", request.Id);

        var dto = _mapper.Map<OrdenServicioDto>(orden);

        return dto with
        {
            Detalles = orden.DetallesOrdenServicio?.Select(d => new DetalleOrdenDto(
                d.Id,
                d.RepuestoId,
                d.Repuesto?.Nombre,
                d.Cantidad,
                d.PrecioUnitario,
                d.Subtotal
            )).ToList(),
            ManosObra = orden.ManosObra?.Select(m => new ManoObraOrdenDto(
                m.Id,
                m.Descripcion,
                m.Costo,
                m.HorasTrabajadas,
                m.EmpleadoId,
                m.Empleado != null ? $"{m.Empleado.Nombres} {m.Empleado.Apellidos}" : null
            )).ToList()
        };
    }
}
