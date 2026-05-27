using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using AutoTallerManager.Application.Features.Vehiculos.Commands;
using AutoTallerManager.Application.Features.Vehiculos.Queries;
using AutoTallerManager.Application.Features.Vehiculos.DTOs;
using AutoTallerManager.Shared.Models;

namespace AutoTallerManager.API.Controllers;

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

    /// <summary>Registra un nuevo vehículo</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<VehiculoDto>), 201)]
    public async Task<IActionResult> Create([FromBody] CreateVehiculoDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateVehiculoCommand(dto), ct);
        return CreatedAtAction(nameof(GetAll), new { }, ApiResponse<VehiculoDto>.Success(result));
    }
}
