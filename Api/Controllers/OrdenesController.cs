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

    /// <summary>Elimina una orden (soft delete — solo si no tiene factura)</summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Policy = "JefeTallerOnly")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteOrdenCommand(id), ct);
        return NoContent();
    }

    /// <summary>Agrega un repuesto/insumo a la orden</summary>
    [HttpPost("{id:guid}/detalles")]
    [Authorize(Policy = "MecanicoOJefe")]
    [ProducesResponseType(typeof(ApiResponse<DetalleOrdenDto>), 201)]
    public async Task<IActionResult> AddDetalle(Guid id, [FromBody] CreateDetalleOrdenDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new AddDetalleOrdenCommand(id, dto), ct);
        return CreatedAtAction(nameof(GetById), new { id }, ApiResponse<DetalleOrdenDto>.Success(result));
    }

    /// <summary>Elimina un repuesto/insumo de la orden</summary>
    [HttpDelete("{id:guid}/detalles/{detalleId:guid}")]
    [Authorize(Policy = "MecanicoOJefe")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> RemoveDetalle(Guid id, Guid detalleId, CancellationToken ct)
    {
        await _mediator.Send(new RemoveDetalleOrdenCommand(id, detalleId), ct);
        return NoContent();
    }

    /// <summary>Agrega mano de obra a la orden</summary>
    [HttpPost("{id:guid}/manos-obra")]
    [Authorize(Policy = "MecanicoOJefe")]
    [ProducesResponseType(typeof(ApiResponse<ManoObraOrdenDto>), 201)]
    public async Task<IActionResult> AddManoObra(Guid id, [FromBody] CreateManoObraOrdenDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new AddManoObraOrdenCommand(id, dto), ct);
        return CreatedAtAction(nameof(GetById), new { id }, ApiResponse<ManoObraOrdenDto>.Success(result));
    }

    /// <summary>Elimina una mano de obra de la orden</summary>
    [HttpDelete("{id:guid}/manos-obra/{manoObraId:guid}")]
    [Authorize(Policy = "MecanicoOJefe")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> RemoveManoObra(Guid id, Guid manoObraId, CancellationToken ct)
    {
        await _mediator.Send(new RemoveManoObraOrdenCommand(id, manoObraId), ct);
        return NoContent();
    }
}
