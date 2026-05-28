using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Application.UseCase.Vehiculos;
using Application.Common;

namespace Api.Controllers;

/// <summary>Gestión de Vehículos</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VehiculosController : ControllerBase
{
    private readonly IMediator _mediator;
    public VehiculosController(IMediator mediator) => _mediator = mediator;

    /// <summary>Obtiene vehículos con filtros y paginación</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<VehiculoDto>>), 200)]
    public async Task<IActionResult> GetAll([FromQuery] VehiculoFiltroDto filtro, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetVehiculosQuery(filtro), ct);
        Response.Headers["X-Total-Count"] = result.TotalCount.ToString();
        return Ok(ApiResponse<PagedResult<VehiculoDto>>.Success(result));
    }

    /// <summary>Obtiene un vehículo por ID</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<VehiculoDto>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetVehiculoByIdQuery(id), ct);
        return Ok(ApiResponse<VehiculoDto>.Success(result));
    }

    /// <summary>Registra un nuevo vehículo</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<VehiculoDto>), 201)]
    public async Task<IActionResult> Create([FromBody] CreateVehiculoDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateVehiculoCommand(dto), ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<VehiculoDto>.Success(result));
    }

    /// <summary>Actualiza un vehículo</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<VehiculoDto>), 200)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateVehiculoDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new UpdateVehiculoCommand(id, dto), ct);
        return Ok(ApiResponse<VehiculoDto>.Success(result));
    }

    /// <summary>Elimina un vehículo (soft delete)</summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteVehiculoCommand(id), ct);
        return NoContent();
    }
}
