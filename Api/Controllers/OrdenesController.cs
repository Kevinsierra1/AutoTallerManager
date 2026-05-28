using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Application.UseCase.Ordenes;
using Application.Common;

namespace Api.Controllers;

/// <summary>Gestión de Órdenes de Servicio</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrdenesController : ControllerBase
{
    private readonly IMediator _mediator;
    public OrdenesController(IMediator mediator) => _mediator = mediator;

    /// <summary>Obtiene órdenes con filtros</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<OrdenServicioDto>>), 200)]
    public async Task<IActionResult> GetAll([FromQuery] OrdenFiltroDto filtro, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetOrdenesQuery(filtro), ct);
        Response.Headers["X-Total-Count"] = result.TotalCount.ToString();
        return Ok(ApiResponse<PagedResult<OrdenServicioDto>>.Success(result));
    }

    /// <summary>Obtiene una orden por ID</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<OrdenServicioDto>), 200)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetOrdenByIdQuery(id), ct);
        return Ok(ApiResponse<OrdenServicioDto>.Success(result));
    }

    /// <summary>Crea una nueva orden de servicio (solo Recepcionista o Jefe de Taller)</summary>
    [HttpPost]
    [Authorize(Policy = "RecepcionOnly")]
    [ProducesResponseType(typeof(ApiResponse<OrdenServicioDto>), 201)]
    public async Task<IActionResult> Create([FromBody] CreateOrdenDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateOrdenCommand(dto), ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<OrdenServicioDto>.Success(result));
    }

    /// <summary>Aprueba una orden para iniciar trabajo (solo Jefe de Taller)</summary>
    [HttpPost("{id:guid}/aprobar")]
    [Authorize(Policy = "JefeTallerOnly")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Aprobar(Guid id, [FromBody] Guid clienteId, CancellationToken ct)
    {
        await _mediator.Send(new AprobarOrdenCommand(id, clienteId), ct);
        return NoContent();
    }

    /// <summary>Asigna mecánico a una orden (Jefe de Taller o Recepcionista)</summary>
    [HttpPost("{id:guid}/asignar-mecanico")]
    [Authorize(Policy = "RecepcionOnly")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> AsignarMecanico(Guid id, [FromBody] Guid empleadoId, CancellationToken ct)
    {
        await _mediator.Send(new AsignarMecanicoCommand(id, empleadoId), ct);
        return NoContent();
    }

    /// <summary>Finaliza una orden de servicio (Jefe de Taller o Mecánico)</summary>
    [HttpPost("{id:guid}/finalizar")]
    [Authorize(Policy = "MecanicoOJefe")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Finalizar(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new FinalizarOrdenCommand(id), ct);
        return NoContent();
    }

    /// <summary>Cancela una orden (Jefe de Taller o Recepcionista)</summary>
    [HttpPost("{id:guid}/cancelar")]
    [Authorize(Policy = "RecepcionOnly")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Cancelar(Guid id, [FromBody] string motivo, CancellationToken ct)
    {
        await _mediator.Send(new CancelarOrdenCommand(id, motivo), ct);
        return NoContent();
    }
}
