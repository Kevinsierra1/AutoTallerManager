using MediatR;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Application.Abstractions;
using Application.Extensions;
using Application.Common;

namespace Application.UseCase.Auditoria;

public record GetAuditoriasQuery(AuditoriaFiltroDto Filtro) : IRequest<PagedResult<AuditoriaDto>>;

public class GetAuditoriasQueryHandler : IRequestHandler<GetAuditoriasQuery, PagedResult<AuditoriaDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAuditoriasQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PagedResult<AuditoriaDto>> Handle(GetAuditoriasQuery request, CancellationToken cancellationToken)
    {
        var q = _context.Auditorias.AsQueryable();
        var f = request.Filtro;
        if (!string.IsNullOrWhiteSpace(f.Entidad)) q = q.Where(a => a.Entidad == f.Entidad);
        if (!string.IsNullOrWhiteSpace(f.UsuarioId)) q = q.Where(a => a.UsuarioId == f.UsuarioId);
        if (!string.IsNullOrWhiteSpace(f.Accion)) q = q.Where(a => a.Accion == f.Accion);
        if (f.FechaDesde.HasValue) q = q.Where(a => a.Fecha >= f.FechaDesde);
        if (f.FechaHasta.HasValue) q = q.Where(a => a.Fecha <= f.FechaHasta);
        return await q.OrderByDescending(a => a.Fecha)
            .ProjectTo<AuditoriaDto>(_mapper.ConfigurationProvider)
            .ToPagedResultAsync(f.PageNumber, f.PageSize, cancellationToken);
    }
}
