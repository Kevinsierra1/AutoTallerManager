using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Application.UseCase.Usuarios;
using Application.Common;

namespace Api.Controllers;

/// <summary>Gestión de Usuarios del sistema</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class UsuariosController : ControllerBase
{
    private readonly IMediator _mediator;
    public UsuariosController(IMediator mediator) => _mediator = mediator;

    /// <summary>Obtiene lista paginada de usuarios con filtros</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<UsuarioDto>>), 200)]
    public async Task<IActionResult> GetAll([FromQuery] UsuarioFiltroDto filtro, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetUsuariosQuery(filtro), ct);
        Response.Headers["X-Total-Count"] = result.TotalCount.ToString();
        return Ok(ApiResponse<PagedResult<UsuarioDto>>.Success(result));
    }

    /// <summary>Obtiene un usuario por ID</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<UsuarioDto>), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetUsuarioByIdQuery(id), ct);
        return Ok(ApiResponse<UsuarioDto>.Success(result));
    }

    /// <summary>Crea un nuevo usuario</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<UsuarioDto>), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Create([FromBody] CreateUsuarioDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateUsuarioCommand(dto), ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<UsuarioDto>.Success(result));
    }

    /// <summary>Actualiza datos de un usuario</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<UsuarioDto>), 200)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUsuarioDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new UpdateUsuarioCommand(id, dto), ct);
        return Ok(ApiResponse<UsuarioDto>.Success(result));
    }

    /// <summary>Elimina un usuario (soft delete)</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteUsuarioCommand(id), ct);
        return NoContent();
    }

    /// <summary>Cambia la contraseña de un usuario</summary>
    [HttpPut("{id:guid}/cambiar-password")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> CambiarPassword(Guid id, [FromBody] CambiarPasswordDto dto, CancellationToken ct)
    {
        await _mediator.Send(new CambiarPasswordCommand(id, dto.NuevaPassword), ct);
        return NoContent();
    }

    /// <summary>Asigna un rol a un usuario</summary>
    [HttpPost("{id:guid}/roles/{rolId:guid}")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> AsignarRol(Guid id, Guid rolId, CancellationToken ct)
    {
        await _mediator.Send(new AsignarRolUsuarioCommand(id, rolId), ct);
        return NoContent();
    }

    /// <summary>Remueve un rol de un usuario</summary>
    [HttpDelete("{id:guid}/roles/{rolId:guid}")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> RemoverRol(Guid id, Guid rolId, CancellationToken ct)
    {
        await _mediator.Send(new RemoverRolUsuarioCommand(id, rolId), ct);
        return NoContent();
    }
}
