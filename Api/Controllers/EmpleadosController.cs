using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Application.UseCase.Empleados;
using Application.Common;
using Domain.Enums;

namespace Api.Controllers;

/// <summary>Gestión de Empleados</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EmpleadosController : ControllerBase
{
    private readonly IMediator _mediator;
    public EmpleadosController(IMediator mediator) => _mediator = mediator;

    /// <summary>Obtiene lista de empleados</summary>
    [HttpGet]
    [Authorize(Roles = "Admin,JefeTaller,Recepcionista,Mecánico,MecanicoDiagnostico,MecanicoArea")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<EmpleadoDto>>), 200)]
    public async Task<IActionResult> GetAll([FromQuery] TipoEmpleadoEnum? tipo, int pageNumber = 1, int pageSize = 10, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetEmpleadosQuery(tipo, pageNumber, pageSize), ct);
        Response.Headers["X-Total-Count"] = result.TotalCount.ToString();
        return Ok(ApiResponse<PagedResult<EmpleadoDto>>.Success(result));
    }

    /// <summary>Obtiene un empleado por ID</summary>
    [HttpGet("{id:guid}")]
    [Authorize(Roles = "Admin,JefeTaller,Recepcionista,Mecánico,MecanicoDiagnostico,MecanicoArea")]
    [ProducesResponseType(typeof(ApiResponse<EmpleadoDto>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetEmpleadoByIdQuery(id), ct);
        return Ok(ApiResponse<EmpleadoDto>.Success(result));
    }

    /// <summary>Registra un nuevo empleado</summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<EmpleadoDto>), 201)]
    public async Task<IActionResult> Create([FromBody] CreateEmpleadoDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateEmpleadoCommand(dto), ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<EmpleadoDto>.Success(result));
    }

    /// <summary>Actualiza un empleado</summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<EmpleadoDto>), 200)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateEmpleadoDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new UpdateEmpleadoCommand(id, dto), ct);
        return Ok(ApiResponse<EmpleadoDto>.Success(result));
    }

    /// <summary>Elimina un empleado (soft delete)</summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteEmpleadoCommand(id), ct);
        return NoContent();
    }
}
