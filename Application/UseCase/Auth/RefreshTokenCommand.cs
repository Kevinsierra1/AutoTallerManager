using MediatR;
using Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCase.Auth;

public record RefreshTokenCommand(RefreshTokenDto Dto) : IRequest<AuthResponseDto>;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponseDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IJwtService _jwtService;

    public RefreshTokenCommandHandler(IApplicationDbContext context, IJwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    public async Task<AuthResponseDto> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var rt = await _context.RefreshTokens
            .Include(r => r.Usuario).ThenInclude(u => u.UsuarioRoles).ThenInclude(ur => ur.Rol)
            .FirstOrDefaultAsync(r => r.Token == request.Dto.RefreshToken && !r.Revocado, cancellationToken)
            ?? throw new UnauthorizedAccessException("Refresh token inválido o revocado.");

        if (rt.Expiracion < DateTime.UtcNow)
            throw new UnauthorizedAccessException("Refresh token expirado.");

        rt.Revocado = true;

        var roles = rt.Usuario.UsuarioRoles.Select(ur => ur.Rol.Nombre).ToArray();
        var newToken = _jwtService.GenerarToken(rt.Usuario, roles);
        var newRefreshToken = _jwtService.GenerarRefreshToken();

        var newRt = new Domain.Entities.RefreshToken
        {
            Token = newRefreshToken,
            UsuarioId = rt.UsuarioId,
            Expiracion = DateTime.UtcNow.AddDays(7),
            Revocado = false
        };
        _context.RefreshTokens.Add(newRt);
        await _context.SaveChangesAsync(cancellationToken);

        return new AuthResponseDto(newToken, newRefreshToken, DateTime.UtcNow.AddHours(1), roles, rt.Usuario.Id, rt.Usuario.Email, rt.Usuario.Nombres, rt.Usuario.Apellidos);
    }
}
