using MediatR;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using AutoTallerManager.Application.Features.Clientes.DTOs;
using AutoTallerManager.Application.Common.Interfaces;
using AutoTallerManager.Shared.Extensions;
using AutoTallerManager.Shared.Models;

namespace AutoTallerManager.Application.Features.Clientes.Queries;

public record GetClientesQuery(ClienteFiltroDto Filtro) : IRequest<PagedResult<ClienteDto>>;

public class GetClientesQueryHandler : IRequestHandler<GetClientesQuery, PagedResult<ClienteDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetClientesQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<PagedResult<ClienteDto>> Handle(GetClientesQuery request, CancellationToken cancellationToken)
    {
        var q = _context.Clientes.AsQueryable();
        var f = request.Filtro;
        if (!string.IsNullOrWhiteSpace(f.Busqueda))
            q = q.Where(c => c.Nombres.Contains(f.Busqueda) || c.Apellidos.Contains(f.Busqueda) || c.NumeroDocumento.Contains(f.Busqueda));
        if (!string.IsNullOrWhiteSpace(f.TipoDocumento))
            q = q.Where(c => c.TipoDocumento != null && c.TipoDocumento.Nombre == f.TipoDocumento);
        return await q.ProjectTo<ClienteDto>(_mapper.ConfigurationProvider).ToPagedResultAsync(f.PageNumber, f.PageSize, cancellationToken);
    }
}
