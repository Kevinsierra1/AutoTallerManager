using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Abstractions;
using Application.Common.Exceptions;
using Domain.Enums;

namespace Application.UseCase.OrdenAreas;

public record GetOrdenAreaByIdQuery(Guid Id) : IRequest<OrdenAreaDetalleDto>;

public class GetOrdenAreaByIdQueryHandler : IRequestHandler<GetOrdenAreaByIdQuery, OrdenAreaDetalleDto>
{
    private readonly IApplicationDbContext _context;
    public GetOrdenAreaByIdQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<OrdenAreaDetalleDto> Handle(GetOrdenAreaByIdQuery request, CancellationToken cancellationToken)
    {
        var a = await _context.OrdenAreas
            .Include(x => x.OrdenServicio)
            .Include(x => x.AreaTaller)
            .Include(x => x.Mecanico)
            .Include(x => x.Detalles!).ThenInclude(d => d.Repuesto)
            .Include(x => x.ManosObra!).ThenInclude(m => m.Tecnico)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("OrdenArea", request.Id);

        return MapToDto(a);
    }

    internal static OrdenAreaDetalleDto MapToDto(Domain.Entities.OrdenArea a) => new(
        a.Id, a.OrdenServicioId, a.OrdenServicio.NumeroOrden,
        a.AreaTallerId, a.AreaTaller.Nombre, a.AreaTaller.Tipo,
        a.MecanicoId, a.Mecanico != null ? $"{a.Mecanico.Nombres} {a.Mecanico.Apellidos}" : null,
        a.Estado, a.Estado.ToString(),
        a.FechaInicio, a.FechaFin,
        a.TotalMateriales, a.TotalManoObra,
        a.Observaciones, a.CreadoEn,
        a.Detalles?.Select(d => new OrdenAreaDetalleItemDto(
            d.Id, d.RepuestoId, d.Repuesto.Nombre, d.Repuesto.Codigo,
            d.Cantidad, d.PrecioUnitario, d.Subtotal, d.Observaciones)).ToList(),
        a.ManosObra?.Select(m => new OrdenAreaManoObraDto(
            m.Id, m.Descripcion, m.HorasTrabajo, m.TarifaHora, m.Total,
            m.TecnicoId, m.Tecnico != null ? $"{m.Tecnico.Nombres} {m.Tecnico.Apellidos}" : null)).ToList()
    );
}
