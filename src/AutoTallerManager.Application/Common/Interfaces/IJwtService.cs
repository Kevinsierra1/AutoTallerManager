using AutoTallerManager.Domain.Entities;

namespace AutoTallerManager.Application.Common.Interfaces;

public interface IJwtService
{
    string GenerarToken(Usuario usuario, string[] roles);
    string GenerarRefreshToken();
    System.Security.Claims.ClaimsPrincipal? ObtenerPrincipalDesdeTokenExpirado(string token);
}
