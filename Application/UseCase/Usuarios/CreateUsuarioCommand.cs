using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Abstractions;
using Application.Common.Exceptions;
using Domain.Entities;
using FluentValidation.Results;

namespace Application.UseCase.Usuarios;

public record CreateUsuarioCommand(CreateUsuarioDto Dto) : IRequest<UsuarioDto>;

public class CreateUsuarioCommandHandler : IRequestHandler<CreateUsuarioCommand, UsuarioDto>
{
    private readonly IApplicationDbContext _context;
    public CreateUsuarioCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<UsuarioDto> Handle(CreateUsuarioCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        if (await _context.Usuarios.AnyAsync(u => u.Email == dto.Email && !u.Eliminado, cancellationToken))
            throw new ValidationException(new[] { new ValidationFailure("Email", "El email ya está registrado.") });

        var usuario = new Usuario
        {
            Id = Guid.NewGuid(),
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Nombres = dto.Nombres,
            Apellidos = dto.Apellidos,
            Activo = true,
            CreadoEn = DateTime.UtcNow
        };

        _context.Usuarios.Add(usuario);

        if (dto.RolIds?.Count > 0)
        {
            foreach (var rolId in dto.RolIds)
                _context.UsuarioRoles.Add(new UsuarioRol { UsuarioId = usuario.Id, RolId = rolId });
        }

        await _context.SaveChangesAsync(cancellationToken);

        var guardado = await _context.Usuarios
            .Include(u => u.UsuarioRoles).ThenInclude(ur => ur.Rol)
            .FirstAsync(u => u.Id == usuario.Id, cancellationToken);

        return new UsuarioDto(
            guardado.Id, guardado.Email, guardado.Nombres, guardado.Apellidos, guardado.Activo,
            guardado.UsuarioRoles.Where(ur => ur.Rol != null).Select(ur => ur.Rol!.Nombre).ToList(),
            guardado.CreadoEn
        );
    }
}
