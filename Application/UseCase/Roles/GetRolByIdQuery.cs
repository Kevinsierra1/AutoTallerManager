using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Abstractions;
using Application.Common.Exceptions;

namespace Application.UseCase.Roles;

public record GetRolByIdQuery(Guid Id) : IRequest<RolDto>;

public class GetRolByIdQueryHandler : IRequestHandler<GetRolByIdQuery, RolDto>
{
    private readonly IApplicationDbContext _context;
    public GetRolByIdQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<RolDto> Handle(GetRolByIdQuery request, CancellationToken cancellationToken)
    {
        var r = await _context.Roles
            .Include(r => r.RolPermisos!).ThenInclude(rp => rp.Permiso)
            .FirstOrDefaultAsync(r => r.Id == request.Id && !r.Eliminado, cancellationToken)
            ?? throw new NotFoundException("Rol", request.Id);

        return new RolDto(
            r.Id, r.Nombre, r.Descripcion,
            r.RolPermisos!.Where(rp => rp.Permiso != null).Select(rp => rp.Permiso!.Nombre).ToList(),
            r.CreadoEn
        );
    }
}
