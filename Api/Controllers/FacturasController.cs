using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Application.UseCase.Facturas;
using Application.Common;

namespace Api.Controllers;

/// <summary>Gestión de Facturación</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FacturasController : ControllerBase
{
    private readonly IMediator _mediator;
    public FacturasController(IMediator mediator) => _mediator = mediator;

    /// <summary>Obtiene facturas paginadas</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<FacturaDto>>), 200)]
    public async Task<IActionResult> GetAll(int pageNumber = 1, int pageSize = 10, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetFacturasQuery(pageNumber, pageSize), ct);
        Response.Headers["X-Total-Count"] = result.TotalCount.ToString();
        return Ok(ApiResponse<PagedResult<FacturaDto>>.Success(result));
    }

    /// <summary>Obtiene una factura por ID</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<FacturaDto>), 200)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetFacturaByIdQuery(id), ct);
        return Ok(ApiResponse<FacturaDto>.Success(result));
    }

    /// <summary>Genera una factura para una orden finalizada</summary>
    [HttpPost("generar")]
    [Authorize(Roles = "Admin,Recepcionista")]
    [ProducesResponseType(typeof(ApiResponse<FacturaDto>), 201)]
    public async Task<IActionResult> Generar([FromBody] GenerarFacturaDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new GenerarFacturaCommand(dto), ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<FacturaDto>.Success(result));
    }
}
