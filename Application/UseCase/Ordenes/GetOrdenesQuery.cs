using MediatR;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Application.Abstractions;
using Application.Extensions;
using Application.Common;

namespace Application.UseCase.Ordenes;

public record GetOrdenesQuery(OrdenFiltroDto Filtro) : IRequest<PagedResult<OrdenServicioDto>>;

public class GetOrdenesQueryHandler : IRequestHandler<GetOrdenesQuery, PagedResult<OrdenServicioDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetOrdenesQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PagedResult<OrdenServicioDto>> Handle(GetOrdenesQuery request, CancellationToken cancellationToken)
    {
        var q = _context.OrdenesServicio.AsQueryable();
        var f = request.Filtro;
        if (f.ClienteId.HasValue) q = q.Where(o => o.ClienteId == f.ClienteId);
        if (f.VehiculoId.HasValue) q = q.Where(o => o.VehiculoId == f.VehiculoId);
        if (f.Estado.HasValue) q = q.Where(o => o.Estado == f.Estado);
        if (f.FechaDesde.HasValue) q = q.Where(o => o.FechaIngreso >= f.FechaDesde);
        if (f.FechaHasta.HasValue) q = q.Where(o => o.FechaIngreso <= f.FechaHasta);
        return await q.ProjectTo<OrdenServicioDto>(_mapper.ConfigurationProvider).ToPagedResultAsync(f.PageNumber, f.PageSize, cancellationToken);
    }
}
