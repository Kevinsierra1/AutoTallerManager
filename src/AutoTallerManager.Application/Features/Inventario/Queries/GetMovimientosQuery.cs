using MediatR;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using AutoTallerManager.Application.Features.Inventario.DTOs;
using AutoTallerManager.Application.Common.Interfaces;
using AutoTallerManager.Shared.Extensions;
using AutoTallerManager.Shared.Models;

namespace AutoTallerManager.Application.Features.Inventario.Queries;

public record GetMovimientosQuery(Guid? RepuestoId, int PageNumber = 1, int PageSize = 10) : IRequest<PagedResult<MovimientoInventarioDto>>;

public class GetMovimientosQueryHandler : IRequestHandler<GetMovimientosQuery, PagedResult<MovimientoInventarioDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetMovimientosQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PagedResult<MovimientoInventarioDto>> Handle(GetMovimientosQuery request, CancellationToken cancellationToken)
    {
        var q = _context.MovimientosInventario.AsQueryable();
        if (request.RepuestoId.HasValue) q = q.Where(m => m.RepuestoId == request.RepuestoId);
        return await q.OrderByDescending(m => m.FechaMovimiento)
            .ProjectTo<MovimientoInventarioDto>(_mapper.ConfigurationProvider)
            .ToPagedResultAsync(request.PageNumber, request.PageSize, cancellationToken);
    }
}
