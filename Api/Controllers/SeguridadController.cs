using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Application.UseCase.Seguridad;
using Application.Common;

namespace Api.Controllers;

/// <summary>Seguridad: historial de accesos, sesiones activas y logs de errores</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class SeguridadController : ControllerBase
{
    private readonly IMediator _mediator;
    public SeguridadController(IMediator mediator) => _mediator = mediator;

    /// <summary>Obtiene el historial de accesos al sistema</summary>
    [HttpGet("historial-accesos")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<HistorialAccesoDto>>), 200)]
    public async Task<IActionResult> GetHistorialAccesos(
        [FromQuery] Guid? usuarioId,
        [FromQuery] bool? soloFallidos,
        int pageNumber = 1, int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetHistorialAccesosQuery(usuarioId, soloFallidos, pageNumber, pageSize), ct);
        Response.Headers["X-Total-Count"] = result.TotalCount.ToString();
        return Ok(ApiResponse<PagedResult<HistorialAccesoDto>>.Success(result));
    }

    /// <summary>Obtiene las sesiones de usuarios</summary>
    [HttpGet("sesiones")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<SesionUsuarioDto>>), 200)]
    public async Task<IActionResult> GetSesiones(
        [FromQuery] Guid? usuarioId,
        [FromQuery] bool? soloActivas,
        int pageNumber = 1, int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetSesionesQuery(usuarioId, soloActivas, pageNumber, pageSize), ct);
        Response.Headers["X-Total-Count"] = result.TotalCount.ToString();
        return Ok(ApiResponse<PagedResult<SesionUsuarioDto>>.Success(result));
    }

    /// <summary>Obtiene los logs de errores del sistema</summary>
    [HttpGet("logs-errores")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<LogErrorDto>>), 200)]
    public async Task<IActionResult> GetLogsErrores(
        [FromQuery] string? busqueda,
        [FromQuery] string? endpoint,
        int pageNumber = 1, int pageSize = 20,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetLogsErroresQuery(busqueda, endpoint, pageNumber, pageSize), ct);
        Response.Headers["X-Total-Count"] = result.TotalCount.ToString();
        return Ok(ApiResponse<PagedResult<LogErrorDto>>.Success(result));
    }
}
