using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Abstractions;

namespace Application.UseCase.Roles;

public record GetRolesQuery : IRequest<List<RolDto>>;

public class GetRolesQueryHandler : IRequestHandler<GetRolesQuery, List<RolDto>>
{
    private readonly IApplicationDbContext _context;
    public GetRolesQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<RolDto>> Handle(GetRolesQuery request, CancellationToken cancellationToken) =>
        await _context.Roles
            .Include(r => r.RolPermisos!).ThenInclude(rp => rp.Permiso)
            .Where(r => !r.Eliminado)
            .OrderBy(r => r.Nombre)
            .Select(r => new RolDto(
                r.Id,
                r.Nombre,
                r.Descripcion,
                r.RolPermisos!.Where(rp => rp.Permiso != null).Select(rp => rp.Permiso!.Nombre).ToList(),
                r.CreadoEn
            ))
            .ToListAsync(cancellationToken);
}
