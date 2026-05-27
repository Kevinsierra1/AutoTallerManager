using MediatR;
using AutoTallerManager.Application.Features.Auth.DTOs;
using AutoTallerManager.Application.Common.Interfaces;
using AutoTallerManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AutoTallerManager.Application.Features.Auth.Commands;

public record RegisterCommand(RegisterDto Dto) : IRequest<AuthResponseDto>;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponseDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IJwtService _jwtService;

    public RegisterCommandHandler(IApplicationDbContext context, IJwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    public async Task<AuthResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        if (await _context.Usuarios.AnyAsync(u => u.Email == request.Dto.Email, cancellationToken))
            throw new Domain.Exceptions.DomainException("El email ya está registrado.");

        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Dto.Password);

        var usuario = new Usuario
        {
            Email = request.Dto.Email,
            PasswordHash = passwordHash,
            Nombres = request.Dto.Nombres,
            Apellidos = request.Dto.Apellidos,
            Activo = true
        };

        // Asignar rol Cliente por defecto
        var rolCliente = await _context.Roles.FirstOrDefaultAsync(r => r.Nombre == "Cliente", cancellationToken);
        if (rolCliente != null)
        {
            usuario.UsuarioRoles = new List<UsuarioRol>
            {
                new UsuarioRol { UsuarioId = usuario.Id, RolId = rolCliente.Id }
            };
        }

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync(cancellationToken);

        var roles = new[] { "Cliente" };
        var token = _jwtService.GenerarToken(usuario, roles);
        var refreshToken = _jwtService.GenerarRefreshToken();

        var rt = new RefreshToken
        {
            Token = refreshToken,
            UsuarioId = usuario.Id,
            Expiracion = DateTime.UtcNow.AddDays(7),
            Revocado = false
        };
        _context.RefreshTokens.Add(rt);
        await _context.SaveChangesAsync(cancellationToken);

        return new AuthResponseDto(token, refreshToken, DateTime.UtcNow.AddHours(1), roles, usuario.Id, usuario.Email, usuario.Nombres, usuario.Apellidos);
    }
}
