using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Abstractions;
using Application.Common;
using Application.Extensions;

namespace Application.UseCase.Usuarios;

public record GetUsuariosQuery(UsuarioFiltroDto Filtro) : IRequest<PagedResult<UsuarioDto>>;

public class GetUsuariosQueryHandler : IRequestHandler<GetUsuariosQuery, PagedResult<UsuarioDto>>
{
    private readonly IApplicationDbContext _context;
    public GetUsuariosQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<PagedResult<UsuarioDto>> Handle(GetUsuariosQuery request, CancellationToken cancellationToken)
    {
        var f = request.Filtro;
        var q = _context.Usuarios
            .Include(u => u.UsuarioRoles).ThenInclude(ur => ur.Rol)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(f.Busqueda))
        {
            var patron = $"%{f.Busqueda.Trim()}%";
            q = q.Where(u => EF.Functions.ILike(u.Email,     patron)
                           || EF.Functions.ILike(u.Nombres,   patron)
                           || EF.Functions.ILike(u.Apellidos, patron));
        }
        if (f.Activo.HasValue)
            q = q.Where(u => u.Activo == f.Activo.Value);
        if (!string.IsNullOrWhiteSpace(f.Rol))
            q = q.Where(u => u.UsuarioRoles.Any(ur => ur.Rol != null && ur.Rol.Nombre == f.Rol));

        var projected = q.OrderBy(u => u.Apellidos).ThenBy(u => u.Nombres)
            .Select(u => new UsuarioDto(
                u.Id,
                u.Email,
                u.Nombres,
                u.Apellidos,
                u.Activo,
                u.UsuarioRoles.Where(ur => ur.Rol != null).Select(ur => ur.Rol!.Nombre).ToList(),
                u.CreadoEn
            ));

        return await projected.ToPagedResultAsync(f.PageNumber, f.PageSize, cancellationToken);
    }
}
