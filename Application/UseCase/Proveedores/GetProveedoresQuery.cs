using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Abstractions;
using Application.Common;
using Application.Extensions;

namespace Application.UseCase.Proveedores;

public record GetProveedoresQuery(ProveedorFiltroDto Filtro) : IRequest<PagedResult<ProveedorDto>>;

public class GetProveedoresQueryHandler : IRequestHandler<GetProveedoresQuery, PagedResult<ProveedorDto>>
{
    private readonly IApplicationDbContext _context;
    public GetProveedoresQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<PagedResult<ProveedorDto>> Handle(GetProveedoresQuery request, CancellationToken cancellationToken)
    {
        var f = request.Filtro;
        var q = _context.Proveedores.AsQueryable();

        if (!string.IsNullOrWhiteSpace(f.Busqueda))
        {
            var patron = $"%{f.Busqueda.Trim()}%";
            q = q.Where(p => EF.Functions.ILike(p.Nombre, patron)
                           || (p.Nit   != null && EF.Functions.ILike(p.Nit,   patron))
                           || (p.Email != null && EF.Functions.ILike(p.Email, patron)));
        }
        if (f.Activo.HasValue)
            q = q.Where(p => p.Activo == f.Activo.Value);

        var projected = q.Where(p => !p.Eliminado).OrderBy(p => p.Nombre)
            .Select(p => new ProveedorDto(p.Id, p.Nombre, p.RazonSocial, p.Nit, p.Telefono, p.Email, p.Direccion, p.Activo, p.CreadoEn));

        return await projected.ToPagedResultAsync(f.PageNumber, f.PageSize, cancellationToken);
    }
}
