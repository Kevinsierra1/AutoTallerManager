using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Application.UseCase.OrdenAreas;
using Application.Common;
using Domain.Enums;

namespace Api.Controllers;

/// <summary>Gestión de Órdenes por Área de Taller</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdenAreasController : ControllerBase
{
    private readonly IMediator _mediator;
    public OrdenAreasController(IMediator mediator) => _mediator = mediator;

    /// <summary>Lista órdenes de área con filtros y paginación</summary>
    [HttpGet]
    [Authorize(Policy = "MecanicoOJefe")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<OrdenAreaDto>>), 200)]
    public async Task<IActionResult> GetAll(
        [FromQuery] Guid? ordenServicioId,
        [FromQuery] TipoArea? tipoArea,
        [FromQuery] EstadoMiniOrden? estado,
        [FromQuery] Guid? mecanicoId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        var result = await _mediator.Send(
            new GetOrdenAreasQuery(ordenServicioId, tipoArea, estado, mecanicoId, pageNumber, pageSize), ct);
        Response.Headers["X-Total-Count"] = result.TotalCount.ToString();
        return Ok(ApiResponse<PagedResult<OrdenAreaDto>>.Success(result));
    }

    /// <summary>Obtiene una orden de área por ID con detalles completos</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<OrdenAreaDetalleDto>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetOrdenAreaByIdQuery(id), ct);
        return Ok(ApiResponse<OrdenAreaDetalleDto>.Success(result));
    }

    /// <summary>Crea una nueva orden de área vinculada a una orden de servicio</summary>
    [HttpPost]
    [Authorize(Policy = "JefeTallerOnly")]
    [ProducesResponseType(typeof(ApiResponse<OrdenAreaDetalleDto>), 201)]
    public async Task<IActionResult> Create([FromBody] CreateOrdenAreaDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateOrdenAreaCommand(dto), ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<OrdenAreaDetalleDto>.Success(result));
    }

    /// <summary>Asigna un mecánico a un área específica</summary>
    [HttpPatch("{id:guid}/mecanico/{mecanicoId:guid}")]
    [Authorize(Policy = "JefeTallerOnly")]
    [ProducesResponseType(typeof(ApiResponse<OrdenAreaDetalleDto>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> AsignarMecanico(Guid id, Guid mecanicoId, CancellationToken ct)
    {
        var result = await _mediator.Send(new AsignarMecanicoAreaCommand(id, mecanicoId), ct);
        return Ok(ApiResponse<OrdenAreaDetalleDto>.Success(result));
    }

    /// <summary>Actualiza el estado de una orden de área</summary>
    [HttpPatch("{id:guid}/estado")]
    [Authorize(Policy = "MecanicoOJefe")]
    [ProducesResponseType(typeof(ApiResponse<OrdenAreaDetalleDto>), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> UpdateEstado(Guid id, [FromBody] UpdateEstadoAreaDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new UpdateEstadoAreaCommand(id, dto), ct);
        return Ok(ApiResponse<OrdenAreaDetalleDto>.Success(result));
    }
}
