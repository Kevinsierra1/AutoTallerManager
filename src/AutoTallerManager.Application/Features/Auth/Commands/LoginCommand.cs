using MediatR;
using AutoTallerManager.Application.Features.Auth.DTOs;
using AutoTallerManager.Application.Common.Interfaces;
using AutoTallerManager.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace AutoTallerManager.Application.Features.Auth.Commands;

public record LoginCommand(LoginDto Dto) : IRequest<AuthResponseDto>;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IJwtService _jwtService;

    public LoginCommandHandler(IApplicationDbContext context, IJwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var usuario = await _context.Usuarios
            .Include(u => u.UsuarioRoles).ThenInclude(ur => ur.Rol)
            .FirstOrDefaultAsync(u => u.Email == request.Dto.Email && !u.Eliminado, cancellationToken)
            ?? throw new UnauthorizedAccessException("Credenciales inválidas.");

        if (!BCrypt.Net.BCrypt.Verify(request.Dto.Password, usuario.PasswordHash))
            throw new UnauthorizedAccessException("Credenciales inválidas.");

        var roles = usuario.UsuarioRoles.Select(ur => ur.Rol.Nombre).ToArray();
        var token = _jwtService.GenerarToken(usuario, roles);
        var refreshToken = _jwtService.GenerarRefreshToken();

        var rt = new Domain.Entities.RefreshToken
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
