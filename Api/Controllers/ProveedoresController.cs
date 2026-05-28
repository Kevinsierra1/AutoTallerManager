using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Application.UseCase.Proveedores;
using Application.Common;

namespace Api.Controllers;

/// <summary>Gestión de Proveedores</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProveedoresController : ControllerBase
{
    private readonly IMediator _mediator;
    public ProveedoresController(IMediator mediator) => _mediator = mediator;

    /// <summary>Obtiene lista paginada de proveedores</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ProveedorDto>>), 200)]
    public async Task<IActionResult> GetAll([FromQuery] ProveedorFiltroDto filtro, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetProveedoresQuery(filtro), ct);
        Response.Headers["X-Total-Count"] = result.TotalCount.ToString();
        return Ok(ApiResponse<PagedResult<ProveedorDto>>.Success(result));
    }

    /// <summary>Obtiene un proveedor por ID</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ProveedorDto>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetProveedorByIdQuery(id), ct);
        return Ok(ApiResponse<ProveedorDto>.Success(result));
    }

    /// <summary>Crea un nuevo proveedor</summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Recepcionista")]
    [ProducesResponseType(typeof(ApiResponse<ProveedorDto>), 201)]
    public async Task<IActionResult> Create([FromBody] CreateProveedorDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateProveedorCommand(dto), ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<ProveedorDto>.Success(result));
    }

    /// <summary>Actualiza un proveedor</summary>
    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,Recepcionista")]
    [ProducesResponseType(typeof(ApiResponse<ProveedorDto>), 200)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProveedorDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new UpdateProveedorCommand(id, dto), ct);
        return Ok(ApiResponse<ProveedorDto>.Success(result));
    }

    /// <summary>Elimina un proveedor (soft delete)</summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteProveedorCommand(id), ct);
        return NoContent();
    }
}
