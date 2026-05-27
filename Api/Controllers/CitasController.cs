using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Application.UseCase.Citas;
using Application.Common;

namespace Api.Controllers;

/// <summary>Gestión de Citas</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CitasController : ControllerBase
{
    private readonly IMediator _mediator;
    public CitasController(IMediator mediator) => _mediator = mediator;

    /// <summary>Obtiene lista paginada de citas</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<CitaDto>>), 200)]
    public async Task<IActionResult> GetAll([FromQuery] CitaFiltroDto filtro, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetCitasQuery(filtro), ct);
        Response.Headers["X-Total-Count"] = result.TotalCount.ToString();
        return Ok(ApiResponse<PagedResult<CitaDto>>.Success(result));
    }

    /// <summary>Obtiene una cita por ID</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<CitaDto>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetCitaByIdQuery(id), ct);
        return Ok(ApiResponse<CitaDto>.Success(result));
    }

    /// <summary>Crea una nueva cita</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<CitaDto>), 201)]
    public async Task<IActionResult> Create([FromBody] CreateCitaDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateCitaCommand(dto), ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<CitaDto>.Success(result));
    }

    /// <summary>Actualiza una cita</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<CitaDto>), 200)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCitaDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new UpdateCitaCommand(id, dto), ct);
        return Ok(ApiResponse<CitaDto>.Success(result));
    }

    /// <summary>Elimina una cita</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteCitaCommand(id), ct);
        return NoContent();
    }
}
