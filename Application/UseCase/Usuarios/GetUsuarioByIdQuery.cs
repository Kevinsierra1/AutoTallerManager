using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Abstractions;
using Application.Common.Exceptions;

namespace Application.UseCase.Usuarios;

public record GetUsuarioByIdQuery(Guid Id) : IRequest<UsuarioDto>;

public class GetUsuarioByIdQueryHandler : IRequestHandler<GetUsuarioByIdQuery, UsuarioDto>
{
    private readonly IApplicationDbContext _context;
    public GetUsuarioByIdQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<UsuarioDto> Handle(GetUsuarioByIdQuery request, CancellationToken cancellationToken)
    {
        var u = await _context.Usuarios
            .Include(u => u.UsuarioRoles).ThenInclude(ur => ur.Rol)
            .FirstOrDefaultAsync(u => u.Id == request.Id && !u.Eliminado, cancellationToken)
            ?? throw new NotFoundException("Usuario", request.Id);

        return new UsuarioDto(
            u.Id, u.Email, u.Nombres, u.Apellidos, u.Activo,
            u.UsuarioRoles.Where(ur => ur.Rol != null).Select(ur => ur.Rol!.Nombre).ToList(),
            u.CreadoEn
        );
    }
}
