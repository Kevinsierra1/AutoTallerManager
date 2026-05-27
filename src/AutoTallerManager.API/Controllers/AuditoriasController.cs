using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using AutoTallerManager.Application.Features.Auditoria.Queries;
using AutoTallerManager.Application.Features.Auditoria.DTOs;
using AutoTallerManager.Shared.Models;

namespace AutoTallerManager.API.Controllers;

/// <summary>Auditoría del Sistema</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AuditoriasController : ControllerBase
{
    private readonly IMediator _mediator;
    public AuditoriasController(IMediator mediator) => _mediator = mediator;

    /// <summary>Obtiene registros de auditoría</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<AuditoriaDto>>), 200)]
    public async Task<IActionResult> GetAll([FromQuery] AuditoriaFiltroDto filtro, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAuditoriasQuery(filtro), ct);
        Response.Headers["X-Total-Count"] = result.TotalCount.ToString();
        return Ok(ApiResponse<PagedResult<AuditoriaDto>>.Success(result));
    }
}
