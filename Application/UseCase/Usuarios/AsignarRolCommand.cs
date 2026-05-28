using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Abstractions;
using Application.Common.Exceptions;
using Domain.Entities;

namespace Application.UseCase.Usuarios;

public record AsignarRolUsuarioCommand(Guid UsuarioId, Guid RolId) : IRequest;

public class AsignarRolUsuarioCommandHandler : IRequestHandler<AsignarRolUsuarioCommand>
{
    private readonly IApplicationDbContext _context;
    public AsignarRolUsuarioCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(AsignarRolUsuarioCommand request, CancellationToken cancellationToken)
    {
        if (!await _context.Usuarios.AnyAsync(u => u.Id == request.UsuarioId && !u.Eliminado, cancellationToken))
            throw new NotFoundException("Usuario", request.UsuarioId);
        if (!await _context.Roles.AnyAsync(r => r.Id == request.RolId && !r.Eliminado, cancellationToken))
            throw new NotFoundException("Rol", request.RolId);
        if (await _context.UsuarioRoles.AnyAsync(ur => ur.UsuarioId == request.UsuarioId && ur.RolId == request.RolId, cancellationToken))
            return;

        _context.UsuarioRoles.Add(new UsuarioRol { UsuarioId = request.UsuarioId, RolId = request.RolId });
        await _context.SaveChangesAsync(cancellationToken);
    }
}

public record RemoverRolUsuarioCommand(Guid UsuarioId, Guid RolId) : IRequest;

public class RemoverRolUsuarioCommandHandler : IRequestHandler<RemoverRolUsuarioCommand>
{
    private readonly IApplicationDbContext _context;
    public RemoverRolUsuarioCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(RemoverRolUsuarioCommand request, CancellationToken cancellationToken)
    {
        var ur = await _context.UsuarioRoles
            .FirstOrDefaultAsync(ur => ur.UsuarioId == request.UsuarioId && ur.RolId == request.RolId, cancellationToken)
            ?? throw new NotFoundException("UsuarioRol", $"{request.UsuarioId}/{request.RolId}");
        _context.UsuarioRoles.Remove(ur);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
