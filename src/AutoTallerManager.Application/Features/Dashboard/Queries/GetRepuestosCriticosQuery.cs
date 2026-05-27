using MediatR;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using AutoTallerManager.Application.Features.Repuestos.DTOs;
using AutoTallerManager.Application.Common.Interfaces;
using AutoTallerManager.Shared.Extensions;
using AutoTallerManager.Shared.Models;

namespace AutoTallerManager.Application.Features.Dashboard.Queries;

public record GetRepuestosCriticosQuery(int PageNumber = 1, int PageSize = 10) : IRequest<PagedResult<RepuestoDto>>;

public class GetRepuestosCriticosQueryHandler : IRequestHandler<GetRepuestosCriticosQuery, PagedResult<RepuestoDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetRepuestosCriticosQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PagedResult<RepuestoDto>> Handle(GetRepuestosCriticosQuery request, CancellationToken cancellationToken)
    {
        return await _context.Repuestos
            .Where(r => r.StockActual <= r.StockMinimo)
            .ProjectTo<RepuestoDto>(_mapper.ConfigurationProvider)
            .ToPagedResultAsync(request.PageNumber, request.PageSize, cancellationToken);
    }
}
