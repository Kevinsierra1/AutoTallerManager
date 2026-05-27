using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Application.Abstractions;

namespace Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UsuarioId
    {
        get
        {
            var value = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(value, out var id) ? id : null;
        }
    }

    public string? Email => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Email);

    public bool EsAutenticado => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    public bool TieneRol(string rol) =>
        _httpContextAccessor.HttpContext?.User?.IsInRole(rol) ?? false;
}
