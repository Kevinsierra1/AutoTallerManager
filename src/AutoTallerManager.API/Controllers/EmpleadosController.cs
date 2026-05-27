using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using AutoTallerManager.Application.Features.Empleados.Commands;
using AutoTallerManager.Application.Features.Empleados.Queries;
using AutoTallerManager.Application.Features.Empleados.DTOs;
using AutoTallerManager.Domain.Enums;
using AutoTallerManager.Shared.Models;

namespace AutoTallerManager.API.Controllers;

/// <summary>Gestión de Empleados</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class EmpleadosController : ControllerBase
{
    private readonly IMediator _mediator;
    public EmpleadosController(IMediator mediator) => _mediator = mediator;

    /// <summary>Obtiene lista de empleados</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<EmpleadoDto>>), 200)]
    public async Task<IActionResult> GetAll([FromQuery] TipoEmpleadoEnum? tipo, int pageNumber = 1, int pageSize = 10, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetEmpleadosQuery(tipo, pageNumber, pageSize), ct);
        Response.Headers["X-Total-Count"] = result.TotalCount.ToString();
        return Ok(ApiResponse<PagedResult<EmpleadoDto>>.Success(result));
    }

    /// <summary>Registra un nuevo empleado</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<EmpleadoDto>), 201)]
    public async Task<IActionResult> Create([FromBody] CreateEmpleadoDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateEmpleadoCommand(dto), ct);
        return CreatedAtAction(nameof(GetAll), new { id = result.Id }, ApiResponse<EmpleadoDto>.Success(result));
    }
}
