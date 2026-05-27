using MediatR;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Application.Abstractions;
using Application.Extensions;
using Application.Common;

namespace Application.UseCase.Repuestos;

public record GetRepuestosQuery(RepuestoFiltroDto Filtro) : IRequest<PagedResult<RepuestoDto>>;

public class GetRepuestosQueryHandler : IRequestHandler<GetRepuestosQuery, PagedResult<RepuestoDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetRepuestosQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PagedResult<RepuestoDto>> Handle(GetRepuestosQuery request, CancellationToken cancellationToken)
    {
        var q = _context.Repuestos.AsQueryable();
        var f = request.Filtro;
        if (!string.IsNullOrWhiteSpace(f.Busqueda))
            q = q.Where(x => x.Nombre.Contains(f.Busqueda) || x.Codigo.Contains(f.Busqueda));
        if (f.CategoriaId.HasValue) q = q.Where(x => x.CategoriaRepuestoId == f.CategoriaId);
        if (f.StockCritico == true) q = q.Where(x => x.StockActual <= x.StockMinimo);
        if (f.Activo.HasValue) q = q.Where(x => x.Activo == f.Activo);
        return await q.ProjectTo<RepuestoDto>(_mapper.ConfigurationProvider).ToPagedResultAsync(f.PageNumber, f.PageSize, cancellationToken);
    }
}
