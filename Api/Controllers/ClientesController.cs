using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Application.UseCase.Clientes;
using Application.Common;

namespace Api.Controllers;

/// <summary>Gestión de Clientes</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ClientesController : ControllerBase
{
    private readonly IMediator _mediator;
    public ClientesController(IMediator mediator) => _mediator = mediator;

    /// <summary>Obtiene clientes con filtros y paginación</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<ClienteDto>>), 200)]
    public async Task<IActionResult> GetAll([FromQuery] ClienteFiltroDto filtro, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetClientesQuery(filtro), ct);
        Response.Headers["X-Total-Count"] = result.TotalCount.ToString();
        return Ok(ApiResponse<PagedResult<ClienteDto>>.Success(result));
    }

    /// <summary>Obtiene el perfil completo de un cliente (órdenes y pre-órdenes pendientes)</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ClientePerfilDto>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetClienteByIdQuery(id), ct);
        return Ok(ApiResponse<ClientePerfilDto>.Success(result));
    }

    /// <summary>Obtiene un cliente por número secuencial (#1, #2, #3…)</summary>
    [HttpGet("numero/{numero:int}")]
    [ProducesResponseType(typeof(ApiResponse<ClienteDto>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetByNumero(int numero, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetClienteByNumeroQuery(numero), ct);
        return Ok(ApiResponse<ClienteDto>.Success(result));
    }

    /// <summary>Crea un nuevo cliente y genera su cuenta de acceso con contraseña temporal</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<ClienteCreadoDto>), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Create([FromBody] CreateClienteDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateClienteCommand(dto), ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<ClienteCreadoDto>.Success(result));
    }

    /// <summary>Actualiza un cliente</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<ClienteDto>), 200)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateClienteDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new UpdateClienteCommand(id, dto), ct);
        return Ok(ApiResponse<ClienteDto>.Success(result));
    }

    /// <summary>Elimina un cliente (soft delete)</summary>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteClienteCommand(id), ct);
        return NoContent();
    }
}
