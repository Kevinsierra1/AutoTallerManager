using Domain.Entities;

namespace Application.Abstractions;

public interface IJwtService
{
    string GenerarToken(Usuario usuario, string[] roles);
    string GenerarRefreshToken();
    System.Security.Claims.ClaimsPrincipal? ObtenerPrincipalDesdeTokenExpirado(string token);
}
