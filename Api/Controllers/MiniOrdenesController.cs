using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.UseCase.MiniOrdenes;
using Application.Common;
using Application.Abstractions;
using System.Security.Claims;

namespace Api.Controllers;

/// <summary>Gestión de Mini-Órdenes — Flujo M-J-C (Mecánico → Jefe → Cliente)</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MiniOrdenesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IApplicationDbContext _context;
    public MiniOrdenesController(IMediator mediator, IApplicationDbContext context)
    {
        _mediator = mediator;
        _context  = context;
    }

    private Guid CurrentUserId =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? Guid.Empty.ToString());

    private string CurrentUserName =>
        $"{User.FindFirstValue(ClaimTypes.GivenName)} {User.FindFirstValue(ClaimTypes.Surname)}".Trim();

    /// <summary>Lista mini-órdenes con filtros y paginación</summary>
    [HttpGet]
    [Authorize]  // Cualquier usuario autenticado — el cliente ve los suyos filtrando por Estado=3
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

    /// <summary>Crea un nuevo presupuesto — Admin, JefeTaller, Mecánico (MecanicoOnly)</summary>
    [HttpPost]
    [Authorize(Policy = "MecanicoOnly")]
    [ProducesResponseType(typeof(ApiResponse<MiniOrdenDto>), 201)]
    public async Task<IActionResult> Create([FromBody] CreatePresupuestoDto dto, CancellationToken ct)
    {
        // Buscar el Empleado cuyo email coincide con el del usuario actual.
        // Los Empleados y Usuarios son entidades separadas; no comparten ID.
        Guid? mecanicoId = null;
        var roles = User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);
        if (roles.Any(r => r is "Mecánico" or "MecanicoDiagnostico" or "MecanicoArea"))
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (!string.IsNullOrEmpty(email))
            {
                var emp = await _context.Empleados
                    .Where(e => e.Email == email && e.Activo)
                    .Select(e => new { e.Id })
                    .FirstOrDefaultAsync(ct);
                mecanicoId = emp?.Id;
            }
        }

        var result = await _mediator.Send(new CreateMiniOrdenCommand(dto, mecanicoId), ct);
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
        // JefeId = null para Admin/JefeTaller sin registro de Empleado (evita FK violation)
        var result = await _mediator.Send(new AprobarRechazarJefeCommand(id, dto, null, CurrentUserName), ct);
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

    /// <summary>Agrega un repuesto al presupuesto</summary>
    [HttpPost("{id:guid}/detalles")]
    [Authorize(Policy = "MecanicoOnly")]
    [ProducesResponseType(typeof(ApiResponse<MiniOrdenDetalleDto>), 201)]
    public async Task<IActionResult> AddDetalle(Guid id, [FromBody] CreateMiniOrdenDetalleDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new AddDetalleMiniOrdenCommand(id, dto), ct);
        return CreatedAtAction(nameof(GetById), new { id }, ApiResponse<MiniOrdenDetalleDto>.Success(result));
    }

    /// <summary>Elimina un repuesto del presupuesto</summary>
    [HttpDelete("{id:guid}/detalles/{detalleId:guid}")]
    [Authorize(Policy = "MecanicoOnly")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> RemoveDetalle(Guid id, Guid detalleId, CancellationToken ct)
    {
        await _mediator.Send(new RemoveDetalleMiniOrdenCommand(id, detalleId), ct);
        return NoContent();
    }

    /// <summary>Agrega mano de obra al presupuesto</summary>
    [HttpPost("{id:guid}/manos-obra")]
    [Authorize(Policy = "MecanicoOnly")]
    [ProducesResponseType(typeof(ApiResponse<MiniOrdenManoObraDto>), 201)]
    public async Task<IActionResult> AddManoObra(Guid id, [FromBody] CreateMiniOrdenManoObraDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new AddManoObraMiniOrdenCommand(id, dto), ct);
        return CreatedAtAction(nameof(GetById), new { id }, ApiResponse<MiniOrdenManoObraDto>.Success(result));
    }

    /// <summary>Elimina mano de obra del presupuesto</summary>
    [HttpDelete("{id:guid}/manos-obra/{manoObraId:guid}")]
    [Authorize(Policy = "MecanicoOnly")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> RemoveManoObra(Guid id, Guid manoObraId, CancellationToken ct)
    {
        await _mediator.Send(new RemoveManoObraMiniOrdenCommand(id, manoObraId), ct);
        return NoContent();
    }
}
