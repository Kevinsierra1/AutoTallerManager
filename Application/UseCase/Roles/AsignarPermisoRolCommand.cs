using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Abstractions;
using Application.Common.Exceptions;
using Domain.Entities;

namespace Application.UseCase.Roles;

public record AsignarPermisoRolCommand(Guid RolId, Guid PermisoId) : IRequest;

public class AsignarPermisoRolCommandHandler : IRequestHandler<AsignarPermisoRolCommand>
{
    private readonly IApplicationDbContext _context;
    public AsignarPermisoRolCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(AsignarPermisoRolCommand request, CancellationToken cancellationToken)
    {
        if (!await _context.Roles.AnyAsync(r => r.Id == request.RolId && !r.Eliminado, cancellationToken))
            throw new NotFoundException("Rol", request.RolId);
        if (!await _context.Permisos.AnyAsync(p => p.Id == request.PermisoId, cancellationToken))
            throw new NotFoundException("Permiso", request.PermisoId);
        if (await _context.RolPermisos.AnyAsync(rp => rp.RolId == request.RolId && rp.PermisoId == request.PermisoId, cancellationToken))
            return;

        _context.RolPermisos.Add(new RolPermiso { RolId = request.RolId, PermisoId = request.PermisoId });
        await _context.SaveChangesAsync(cancellationToken);
    }
}

public record RemoverPermisoRolCommand(Guid RolId, Guid PermisoId) : IRequest;

public class RemoverPermisoRolCommandHandler : IRequestHandler<RemoverPermisoRolCommand>
{
    private readonly IApplicationDbContext _context;
    public RemoverPermisoRolCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(RemoverPermisoRolCommand request, CancellationToken cancellationToken)
    {
        var rp = await _context.RolPermisos
            .FirstOrDefaultAsync(rp => rp.RolId == request.RolId && rp.PermisoId == request.PermisoId, cancellationToken)
            ?? throw new NotFoundException("RolPermiso", $"{request.RolId}/{request.PermisoId}");
        _context.RolPermisos.Remove(rp);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
