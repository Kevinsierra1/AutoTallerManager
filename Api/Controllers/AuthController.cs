using Microsoft.AspNetCore.Mvc;
using MediatR;
using Application.UseCase.Auth;
using Application.Common;

namespace Api.Controllers;

/// <summary>Autenticación y Autorización</summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    public AuthController(IMediator mediator) => _mediator = mediator;

    /// <summary>Iniciar sesión</summary>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), 200)]
    [ProducesResponseType(401)]
    public async Task<IActionResult> Login([FromBody] LoginDto dto, CancellationToken ct)
    {
        try
        {
            var result = await _mediator.Send(new LoginCommand(dto), ct);
            return Ok(ApiResponse<AuthResponseDto>.Success(result));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ApiResponse<string>.Fail(ex.Message));
        }
    }

    /// <summary>Registrar nuevo usuario</summary>
    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new RegisterCommand(dto), ct);
        return Ok(ApiResponse<AuthResponseDto>.Success(result));
    }

    /// <summary>Registro completo de cliente: crea usuario + perfil + vehículo</summary>
    [HttpPost("register-cliente")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), 200)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> RegisterCliente([FromBody] RegisterClienteDto dto, CancellationToken ct)
    {
        try
        {
            var result = await _mediator.Send(new RegisterClienteCommand(dto), ct);
            return Ok(ApiResponse<AuthResponseDto>.Success(result));
        }
        catch (Domain.Exceptions.DomainException ex)
        {
            return BadRequest(ApiResponse<string>.Fail(ex.Message));
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
        {
            var inner = ex.InnerException?.Message ?? ex.Message;
            var msg = inner.Contains("23505") || inner.Contains("unique")
                ? "El email o la placa ya están registrados."
                : "Error al guardar los datos. Verifica la información e intenta de nuevo.";
            return BadRequest(ApiResponse<string>.Fail(msg));
        }
    }

    /// <summary>Renovar token de acceso</summary>
    [HttpPost("refresh-token")]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), 200)]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto dto, CancellationToken ct)
    {
        try
        {
            var result = await _mediator.Send(new RefreshTokenCommand(dto), ct);
            return Ok(ApiResponse<AuthResponseDto>.Success(result));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ApiResponse<string>.Fail(ex.Message));
        }
    }
}
