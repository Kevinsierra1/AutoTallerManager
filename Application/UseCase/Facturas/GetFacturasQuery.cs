using MediatR;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Application.Abstractions;
using Application.Extensions;
using Application.Common;

namespace Application.UseCase.Facturas;

public record GetFacturasQuery(int PageNumber = 1, int PageSize = 10) : IRequest<PagedResult<FacturaDto>>;

public class GetFacturasQueryHandler : IRequestHandler<GetFacturasQuery, PagedResult<FacturaDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetFacturasQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PagedResult<FacturaDto>> Handle(GetFacturasQuery request, CancellationToken cancellationToken)
    {
        return await _context.Facturas
            .Include(f => f.Cliente)
            .Include(f => f.OrdenServicio)
            .Include(f => f.Ordenes)
            .OrderByDescending(f => f.FechaEmision)
            .ProjectTo<FacturaDto>(_mapper.ConfigurationProvider)
            .ToPagedResultAsync(request.PageNumber, request.PageSize, cancellationToken);
    }
}
