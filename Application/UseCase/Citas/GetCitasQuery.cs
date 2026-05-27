using MediatR;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Application.Abstractions;
using Application.Extensions;
using Application.Common;

namespace Application.UseCase.Citas;

public record GetCitasQuery(CitaFiltroDto Filtro) : IRequest<PagedResult<CitaDto>>;

public class GetCitasQueryHandler : IRequestHandler<GetCitasQuery, PagedResult<CitaDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetCitasQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PagedResult<CitaDto>> Handle(GetCitasQuery request, CancellationToken cancellationToken)
    {
        var q = _context.Citas.Include(c => c.Cliente).Include(c => c.Vehiculo).AsQueryable();
        var f = request.Filtro;
        if (f.ClienteId.HasValue) q = q.Where(x => x.ClienteId == f.ClienteId);
        if (f.VehiculoId.HasValue) q = q.Where(x => x.VehiculoId == f.VehiculoId);
        if (f.Estado.HasValue) q = q.Where(x => x.Estado == f.Estado);
        if (f.FechaDesde.HasValue) q = q.Where(x => x.FechaHora >= f.FechaDesde);
        if (f.FechaHasta.HasValue) q = q.Where(x => x.FechaHora <= f.FechaHasta);
        return await q.ProjectTo<CitaDto>(_mapper.ConfigurationProvider).ToPagedResultAsync(f.PageNumber, f.PageSize, cancellationToken);
    }
}
