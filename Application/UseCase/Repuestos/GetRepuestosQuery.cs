using MediatR;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
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
        {
            var patron = $"%{f.Busqueda.Trim()}%";
            q = q.Where(x => EF.Functions.ILike(x.Nombre, patron)
                           || EF.Functions.ILike(x.Codigo, patron)
                           || (x.Descripcion != null && EF.Functions.ILike(x.Descripcion, patron)));
        }
        if (f.CategoriaId.HasValue) q = q.Where(x => x.CategoriaRepuestoId == f.CategoriaId);
        if (f.StockCritico == true) q = q.Where(x => x.StockActual <= x.StockMinimo);
        if (f.Activo.HasValue) q = q.Where(x => x.Activo == f.Activo);
        return await q.ProjectTo<RepuestoDto>(_mapper.ConfigurationProvider).ToPagedResultAsync(f.PageNumber, f.PageSize, cancellationToken);
    }
}
