using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Abstractions;
using Application.Common;
using Application.Extensions;
using Domain.Enums;

namespace Application.UseCase.OrdenAreas;

public record GetOrdenAreasQuery(
    Guid? OrdenServicioId,
    TipoArea? TipoArea,
    EstadoMiniOrden? Estado,
    Guid? MecanicoId,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<PagedResult<OrdenAreaDto>>;

public class GetOrdenAreasQueryHandler : IRequestHandler<GetOrdenAreasQuery, PagedResult<OrdenAreaDto>>
{
    private readonly IApplicationDbContext _context;
    public GetOrdenAreasQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<PagedResult<OrdenAreaDto>> Handle(GetOrdenAreasQuery request, CancellationToken cancellationToken)
    {
        var q = _context.OrdenAreas
            .Include(x => x.OrdenServicio)
            .Include(x => x.AreaTaller)
            .Include(x => x.Mecanico)
            .AsQueryable();

        if (request.OrdenServicioId.HasValue)
            q = q.Where(x => x.OrdenServicioId == request.OrdenServicioId.Value);
        if (request.TipoArea.HasValue)
            q = q.Where(x => x.AreaTaller.Tipo == request.TipoArea.Value);
        if (request.Estado.HasValue)
            q = q.Where(x => x.Estado == request.Estado.Value);
        if (request.MecanicoId.HasValue)
            q = q.Where(x => x.MecanicoId == request.MecanicoId.Value);

        var projected = q.OrderByDescending(x => x.CreadoEn)
            .Select(x => new OrdenAreaDto(
                x.Id, x.OrdenServicioId, x.OrdenServicio.NumeroOrden,
                x.AreaTallerId, x.AreaTaller.Nombre, x.AreaTaller.Tipo,
                x.MecanicoId,
                x.Mecanico != null ? $"{x.Mecanico.Nombres} {x.Mecanico.Apellidos}" : null,
                x.Estado, x.FechaInicio, x.FechaFin,
                x.TotalMateriales, x.TotalManoObra, x.Observaciones, x.CreadoEn
            ));

        return await projected.ToPagedResultAsync(request.PageNumber, request.PageSize, cancellationToken);
    }
}
