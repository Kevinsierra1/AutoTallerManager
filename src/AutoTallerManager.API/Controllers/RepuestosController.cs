using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using AutoTallerManager.Application.Features.Repuestos.Commands;
using AutoTallerManager.Application.Features.Repuestos.Queries;
using AutoTallerManager.Application.Features.Repuestos.DTOs;
using AutoTallerManager.Shared.Models;

namespace AutoTallerManager.API.Controllers;

/// <summary>Gestión de Repuestos</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class RepuestosController : ControllerBase
{
    private readonly IMediator _mediator;
    public RepuestosController(IMediator mediator) => _mediator = mediator;

    /// <summary>Obtiene lista paginada de repuestos</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<RepuestoDto>>), 200)]
    public async Task<IActionResult> GetAll([FromQuery] RepuestoFiltroDto filtro, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetRepuestosQuery(filtro), ct);
        Response.Headers["X-Total-Count"] = result.TotalCount.ToString();
        return Ok(ApiResponse<PagedResult<RepuestoDto>>.Success(result));
    }

    /// <summary>Obtiene un repuesto por ID</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<RepuestoDto>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetRepuestoByIdQuery(id), ct);
        return Ok(ApiResponse<RepuestoDto>.Success(result));
    }

    /// <summary>Crea un nuevo repuesto</summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Recepcionista")]
    [ProducesResponseType(typeof(ApiResponse<RepuestoDto>), 201)]
    public async Task<IActionResult> Create([FromBody] CreateRepuestoDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateRepuestoCommand(dto), ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<RepuestoDto>.Success(result));
    }

    /// <summary>Actualiza un repuesto</summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Recepcionista")]
    [ProducesResponseType(typeof(ApiResponse<RepuestoDto>), 200)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRepuestoDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new UpdateRepuestoCommand(id, dto), ct);
        return Ok(ApiResponse<RepuestoDto>.Success(result));
    }

    /// <summary>Elimina un repuesto</summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteRepuestoCommand(id), ct);
        return NoContent();
    }
}
