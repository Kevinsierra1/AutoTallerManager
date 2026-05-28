using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Application.UseCase.Roles;
using Application.Common;

namespace Api.Controllers;

/// <summary>Gestión de Roles y Permisos</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class RolesController : ControllerBase
{
    private readonly IMediator _mediator;
    public RolesController(IMediator mediator) => _mediator = mediator;

    /// <summary>Lista todos los roles con sus permisos asignados</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<List<RolDto>>), 200)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetRolesQuery(), ct);
        return Ok(ApiResponse<List<RolDto>>.Success(result));
    }

    /// <summary>Obtiene un rol por ID</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<RolDto>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetRolByIdQuery(id), ct);
        return Ok(ApiResponse<RolDto>.Success(result));
    }

    /// <summary>Crea un nuevo rol</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<RolDto>), 201)]
    public async Task<IActionResult> Create([FromBody] CreateRolDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateRolCommand(dto), ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<RolDto>.Success(result));
    }

    /// <summary>Actualiza un rol</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<RolDto>), 200)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRolDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new UpdateRolCommand(id, dto), ct);
        return Ok(ApiResponse<RolDto>.Success(result));
    }

    /// <summary>Elimina un rol</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteRolCommand(id), ct);
        return NoContent();
    }

    /// <summary>Asigna un permiso a un rol</summary>
    [HttpPost("{id:guid}/permisos/{permisoId:guid}")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> AsignarPermiso(Guid id, Guid permisoId, CancellationToken ct)
    {
        await _mediator.Send(new AsignarPermisoRolCommand(id, permisoId), ct);
        return NoContent();
    }

    /// <summary>Remueve un permiso de un rol</summary>
    [HttpDelete("{id:guid}/permisos/{permisoId:guid}")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> RemoverPermiso(Guid id, Guid permisoId, CancellationToken ct)
    {
        await _mediator.Send(new RemoverPermisoRolCommand(id, permisoId), ct);
        return NoContent();
    }
}
