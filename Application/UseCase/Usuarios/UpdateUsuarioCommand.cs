using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Abstractions;
using Application.Common.Exceptions;

namespace Application.UseCase.Usuarios;

public record UpdateUsuarioCommand(Guid Id, UpdateUsuarioDto Dto) : IRequest<UsuarioDto>;

public class UpdateUsuarioCommandHandler : IRequestHandler<UpdateUsuarioCommand, UsuarioDto>
{
    private readonly IApplicationDbContext _context;
    public UpdateUsuarioCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<UsuarioDto> Handle(UpdateUsuarioCommand request, CancellationToken cancellationToken)
    {
        var usuario = await _context.Usuarios
            .Include(u => u.UsuarioRoles).ThenInclude(ur => ur.Rol)
            .FirstOrDefaultAsync(u => u.Id == request.Id && !u.Eliminado, cancellationToken)
            ?? throw new NotFoundException("Usuario", request.Id);

        var dto = request.Dto;
        if (dto.Nombres != null) usuario.Nombres = dto.Nombres;
        if (dto.Apellidos != null) usuario.Apellidos = dto.Apellidos;
        if (dto.Email != null) usuario.Email = dto.Email;
        if (dto.Activo.HasValue) usuario.Activo = dto.Activo.Value;
        usuario.ActualizadoEn = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return new UsuarioDto(
            usuario.Id, usuario.Email, usuario.Nombres, usuario.Apellidos, usuario.Activo,
            usuario.UsuarioRoles.Where(ur => ur.Rol != null).Select(ur => ur.Rol!.Nombre).ToList(),
            usuario.CreadoEn
        );
    }
}
