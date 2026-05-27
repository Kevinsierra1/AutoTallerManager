using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Application.UseCase.Catalogos;
using Application.Common;

namespace Api.Controllers;

/// <summary>Catálogos auxiliares: Marcas, Modelos de vehículo, Colores</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CatalogosController : ControllerBase
{
    private readonly IMediator _mediator;
    public CatalogosController(IMediator mediator) => _mediator = mediator;

    /// <summary>Lista todas las marcas de vehículos</summary>
    [HttpGet("marcas")]
    public async Task<IActionResult> GetMarcas(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetMarcasQuery(), ct);
        return Ok(ApiResponse<List<CatalogoItemDto>>.Success(result));
    }

    /// <summary>Lista modelos de vehículos, opcionalmente filtrados por marca</summary>
    [HttpGet("modelos")]
    public async Task<IActionResult> GetModelos([FromQuery] Guid? marcaId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetModelosQuery(marcaId), ct);
        return Ok(ApiResponse<List<ModeloItemDto>>.Success(result));
    }

    /// <summary>Lista todos los colores disponibles</summary>
    [HttpGet("colores")]
    public async Task<IActionResult> GetColores(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetColoresQuery(), ct);
        return Ok(ApiResponse<List<ColorItemDto>>.Success(result));
    }
}
