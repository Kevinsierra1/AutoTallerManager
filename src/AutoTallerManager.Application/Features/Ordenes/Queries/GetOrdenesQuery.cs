using MediatR;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using AutoTallerManager.Application.Features.Ordenes.DTOs;
using AutoTallerManager.Application.Common.Interfaces;
using AutoTallerManager.Shared.Extensions;
using AutoTallerManager.Shared.Models;

namespace AutoTallerManager.Application.Features.Ordenes.Queries;

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
        var q = _context.OrdenesServicio
            .Include(o => o.Cliente)
            .Include(o => o.Vehiculo)
            .AsQueryable();
        var f = request.Filtro;
        if (f.ClienteId.HasValue) q = q.Where(o => o.ClienteId == f.ClienteId);
        if (f.VehiculoId.HasValue) q = q.Where(o => o.VehiculoId == f.VehiculoId);
        if (f.Estado.HasValue) q = q.Where(o => o.Estado == f.Estado);
        if (f.FechaDesde.HasValue) q = q.Where(o => o.FechaIngreso >= f.FechaDesde);
        if (f.FechaHasta.HasValue) q = q.Where(o => o.FechaIngreso <= f.FechaHasta);
        return await q.ProjectTo<OrdenServicioDto>(_mapper.ConfigurationProvider).ToPagedResultAsync(f.PageNumber, f.PageSize, cancellationToken);
    }
}
