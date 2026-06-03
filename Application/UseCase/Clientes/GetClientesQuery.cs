using MediatR;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Application.Abstractions;
using Application.Extensions;
using Application.Common;

namespace Application.UseCase.Clientes;

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
        {
            var raw    = f.Busqueda.Trim().TrimStart('#');
            var patron = $"%{raw}%";
            if (int.TryParse(raw, out var numero))
                q = q.Where(c => c.Numero == numero
                    || EF.Functions.ILike(c.Nombres,         patron)
                    || EF.Functions.ILike(c.Apellidos,       patron)
                    || EF.Functions.ILike(c.NumeroDocumento, patron));
            else
                q = q.Where(c => EF.Functions.ILike(c.Nombres,         patron)
                    || EF.Functions.ILike(c.Apellidos,       patron)
                    || EF.Functions.ILike(c.NumeroDocumento, patron)
                    || (c.Email != null && EF.Functions.ILike(c.Email,  patron)));
        }
        if (!string.IsNullOrWhiteSpace(f.TipoDocumento))
            q = q.Where(c => c.TipoDocumento != null && c.TipoDocumento.Nombre == f.TipoDocumento);
        return await q.ProjectTo<ClienteDto>(_mapper.ConfigurationProvider).ToPagedResultAsync(f.PageNumber, f.PageSize, cancellationToken);
    }
}
