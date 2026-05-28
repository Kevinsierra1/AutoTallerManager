using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Application.UseCase.Notificaciones;
using Application.Abstractions;
using Application.Common;

namespace Api.Controllers;

/// <summary>Notificaciones del usuario autenticado</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificacionesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;

    public NotificacionesController(IMediator mediator, ICurrentUserService currentUser)
    {
        _mediator = mediator;
        _currentUser = currentUser;
    }

    /// <summary>Obtiene las notificaciones del usuario autenticado</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<NotificacionDto>>), 200)]
    public async Task<IActionResult> GetMias([FromQuery] bool? soloNoLeidas, int pageNumber = 1, int pageSize = 20, CancellationToken ct = default)
    {
        var usuarioId = _currentUser.UsuarioId ?? throw new UnauthorizedAccessException();
        var result = await _mediator.Send(new GetNotificacionesQuery(usuarioId, soloNoLeidas, pageNumber, pageSize), ct);
        Response.Headers["X-Total-Count"] = result.TotalCount.ToString();
        return Ok(ApiResponse<PagedResult<NotificacionDto>>.Success(result));
    }

    /// <summary>Marca una notificación como leída</summary>
    [HttpPut("{id:guid}/leer")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> MarcarLeida(Guid id, CancellationToken ct)
    {
        var usuarioId = _currentUser.UsuarioId ?? throw new UnauthorizedAccessException();
        await _mediator.Send(new MarcarLeidaCommand(id, usuarioId), ct);
        return NoContent();
    }

    /// <summary>Marca todas las notificaciones del usuario como leídas</summary>
    [HttpPut("leer-todas")]
    [ProducesResponseType(typeof(ApiResponse<int>), 200)]
    public async Task<IActionResult> MarcarTodasLeidas(CancellationToken ct)
    {
        var usuarioId = _currentUser.UsuarioId ?? throw new UnauthorizedAccessException();
        var count = await _mediator.Send(new MarcarTodasLeidasCommand(usuarioId), ct);
        return Ok(ApiResponse<int>.Success(count));
    }
}
