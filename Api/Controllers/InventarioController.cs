using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Application.UseCase.Inventario;
using Application.Common;

namespace Api.Controllers;

/// <summary>Gestión de Inventario</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class InventarioController : ControllerBase
{
    private readonly IMediator _mediator;
    public InventarioController(IMediator mediator) => _mediator = mediator;

    /// <summary>Obtiene movimientos de inventario</summary>
    [HttpGet("movimientos")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<MovimientoInventarioDto>>), 200)]
    public async Task<IActionResult> GetMovimientos([FromQuery] Guid? repuestoId, int pageNumber = 1, int pageSize = 10, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetMovimientosQuery(repuestoId, pageNumber, pageSize), ct);
        Response.Headers["X-Total-Count"] = result.TotalCount.ToString();
        return Ok(ApiResponse<PagedResult<MovimientoInventarioDto>>.Success(result));
    }

    /// <summary>Registra entrada de inventario</summary>
    [HttpPost("entrada")]
    [Authorize(Roles = "Admin,Recepcionista")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Entrada([FromBody] EntradaInventarioDto dto, CancellationToken ct)
    {
        await _mediator.Send(new EntradaInventarioCommand(dto), ct);
        return NoContent();
    }

    /// <summary>Registra salida de inventario</summary>
    [HttpPost("salida")]
    [Authorize(Roles = "Admin,Mecánico,Recepcionista")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Salida([FromBody] SalidaInventarioDto dto, CancellationToken ct)
    {
        await _mediator.Send(new SalidaInventarioCommand(dto), ct);
        return NoContent();
    }
}
