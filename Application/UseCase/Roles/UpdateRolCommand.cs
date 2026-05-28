using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Abstractions;
using Application.Common.Exceptions;

namespace Application.UseCase.Roles;

public record UpdateRolCommand(Guid Id, UpdateRolDto Dto) : IRequest<RolDto>;

public class UpdateRolCommandHandler : IRequestHandler<UpdateRolCommand, RolDto>
{
    private readonly IApplicationDbContext _context;
    public UpdateRolCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<RolDto> Handle(UpdateRolCommand request, CancellationToken cancellationToken)
    {
        var rol = await _context.Roles
            .Include(r => r.RolPermisos!).ThenInclude(rp => rp.Permiso)
            .FirstOrDefaultAsync(r => r.Id == request.Id && !r.Eliminado, cancellationToken)
            ?? throw new NotFoundException("Rol", request.Id);

        if (request.Dto.Nombre != null) rol.Nombre = request.Dto.Nombre;
        if (request.Dto.Descripcion != null) rol.Descripcion = request.Dto.Descripcion;
        rol.ActualizadoEn = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return new RolDto(
            rol.Id, rol.Nombre, rol.Descripcion,
            rol.RolPermisos!.Where(rp => rp.Permiso != null).Select(rp => rp.Permiso!.Nombre).ToList(),
            rol.CreadoEn
        );
    }
}
