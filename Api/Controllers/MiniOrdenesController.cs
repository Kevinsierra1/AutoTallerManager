using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Application.UseCase.MiniOrdenes;
using Application.Common;
using System.Security.Claims;

namespace Api.Controllers;

/// <summary>Gestión de Mini-Órdenes — Flujo M-J-C (Mecánico → Jefe → Cliente)</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MiniOrdenesController : ControllerBase
{
    private readonly IMediator _mediator;
    public MiniOrdenesController(IMediator mediator) => _mediator = mediator;

    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? Guid.Empty.ToString());

    private string CurrentUserName =>
        $"{User.FindFirstValue(ClaimTypes.GivenName)} {User.FindFirstValue(ClaimTypes.Surname)}".Trim();

    /// <summary>Lista mini-órdenes con filtros y paginación</summary>
    [HttpGet]
    [Authorize(Policy = "MecanicoOJefe")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<MiniOrdenDto>>), 200)]
    public async Task<IActionResult> GetAll([FromQuery] MiniOrdenFiltroDto filtro, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetMiniOrdenesQuery(filtro), ct);
        Response.Headers["X-Total-Count"] = result.TotalCount.ToString();
        return Ok(ApiResponse<PagedResult<MiniOrdenDto>>.Success(result));
    }

    /// <summary>Obtiene una mini-orden por ID con sus detalles completos</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<MiniOrdenDto>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetMiniOrdenByIdQuery(id), ct);
        return Ok(ApiResponse<MiniOrdenDto>.Success(result));
    }

    /// <summary>Crea un nuevo presupuesto — Cliente + Vehículo (solo Mecánicos)</summary>
    [HttpPost]
    [Authorize(Policy = "MecanicoOnly")]
    [ProducesResponseType(typeof(ApiResponse<MiniOrdenDto>), 201)]
    public async Task<IActionResult> Create([FromBody] CreatePresupuestoDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateMiniOrdenCommand(dto, CurrentUserId), ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<MiniOrdenDto>.Success(result));
    }

    /// <summary>Envía la mini-orden a revisión del Jefe de Taller (Paso 1 del Flujo M-J-C)</summary>
    [HttpPost("{id:guid}/enviar-revision")]
    [Authorize(Policy = "MecanicoOnly")]
    [ProducesResponseType(typeof(ApiResponse<MiniOrdenDto>), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> EnviarRevision(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new EnviarRevisionJefeCommand(id), ct);
        return Ok(ApiResponse<MiniOrdenDto>.Success(result));
    }

    /// <summary>Aprueba o rechaza la mini-orden como Jefe de Taller (Paso 2 del Flujo M-J-C)</summary>
    [HttpPost("{id:guid}/aprobacion-jefe")]
    [Authorize(Policy = "JefeTallerOnly")]
    [ProducesResponseType(typeof(ApiResponse<MiniOrdenDto>), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> AprobarRechazarJefe(Guid id, [FromBody] AprobarRechazarMiniOrdenDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new AprobarRechazarJefeCommand(id, dto, CurrentUserId, CurrentUserName), ct);
        return Ok(ApiResponse<MiniOrdenDto>.Success(result));
    }

    /// <summary>Aprueba o rechaza la mini-orden como Cliente (Paso 3 del Flujo M-J-C)</summary>
    [HttpPost("{id:guid}/aprobacion-cliente")]
    [Authorize(Policy = "ClienteOrAdmin")]
    [ProducesResponseType(typeof(ApiResponse<MiniOrdenDto>), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> AprobarRechazarCliente(Guid id, [FromBody] AprobarRechazarMiniOrdenDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new AprobarRechazarClienteCommand(id, dto, CurrentUserName), ct);
        return Ok(ApiResponse<MiniOrdenDto>.Success(result));
    }

    /// <summary>Marca la mini-orden como completada (Mecánico o Jefe)</summary>
    [HttpPost("{id:guid}/completar")]
    [Authorize(Policy = "MecanicoOJefe")]
    [ProducesResponseType(typeof(ApiResponse<MiniOrdenDto>), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Completar(Guid id, [FromQuery] string? observacion, CancellationToken ct)
    {
        var result = await _mediator.Send(new CompletarMiniOrdenCommand(id, observacion), ct);
        return Ok(ApiResponse<MiniOrdenDto>.Success(result));
    }
}
