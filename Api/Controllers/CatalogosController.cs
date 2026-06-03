using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Application.UseCase.Catalogos;
using Application.Common;

namespace Api.Controllers;

/// <summary>Catálogos del sistema: marcas, modelos, estados, combustibles, transmisiones, prioridades, permisos y configuraciones</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CatalogosController : ControllerBase
{
    private readonly IMediator _mediator;
    public CatalogosController(IMediator mediator) => _mediator = mediator;

    // ── Vehículos ─────────────────────────────────────────────────────────────

    /// <summary>Lista todas las marcas de vehículos</summary>
    [HttpGet("marcas")]
    [AllowAnonymous]
    public async Task<IActionResult> GetMarcas(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetMarcasQuery(), ct);
        return Ok(ApiResponse<List<CatalogoItemDto>>.Success(result));
    }

    /// <summary>Lista modelos de vehículos, opcionalmente filtrados por marca</summary>
    [HttpGet("modelos")]
    [AllowAnonymous]
    public async Task<IActionResult> GetModelos([FromQuery] Guid? marcaId, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetModelosQuery(marcaId), ct);
        return Ok(ApiResponse<List<ModeloItemDto>>.Success(result));
    }

    /// <summary>Lista todos los colores disponibles</summary>
    [HttpGet("colores")]
    [AllowAnonymous]
    public async Task<IActionResult> GetColores(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetColoresQuery(), ct);
        return Ok(ApiResponse<List<ColorItemDto>>.Success(result));
    }

    /// <summary>Lista los tipos de combustible</summary>
    [HttpGet("tipos-combustible")]
    public async Task<IActionResult> GetTiposCombustible(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetTiposCombustibleQuery(), ct);
        return Ok(ApiResponse<List<CatalogoItemDto>>.Success(result));
    }

    /// <summary>Lista los tipos de transmisión</summary>
    [HttpGet("tipos-transmision")]
    public async Task<IActionResult> GetTiposTransmision(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetTiposTransmisionQuery(), ct);
        return Ok(ApiResponse<List<CatalogoItemDto>>.Success(result));
    }

    // ── Órdenes de Servicio ───────────────────────────────────────────────────

    /// <summary>Lista los estados de órdenes de servicio</summary>
    [HttpGet("estados-orden")]
    public async Task<IActionResult> GetEstadosOrden(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetEstadosOrdenQuery(), ct);
        return Ok(ApiResponse<List<EstadoOrdenItemDto>>.Success(result));
    }

    /// <summary>Lista las prioridades de órdenes</summary>
    [HttpGet("prioridades-orden")]
    public async Task<IActionResult> GetPrioridadesOrden(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetPrioridadesOrdenQuery(), ct);
        return Ok(ApiResponse<List<PrioridadOrdenItemDto>>.Success(result));
    }

    // ── Citas ─────────────────────────────────────────────────────────────────

    /// <summary>Lista los estados de citas</summary>
    [HttpGet("estados-cita")]
    public async Task<IActionResult> GetEstadosCita(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetEstadosCitaQuery(), ct);
        return Ok(ApiResponse<List<CatalogoItemDto>>.Success(result));
    }

    // ── Facturación ───────────────────────────────────────────────────────────

    /// <summary>Lista los estados de facturas</summary>
    [HttpGet("estados-factura")]
    public async Task<IActionResult> GetEstadosFactura(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetEstadosFacturaQuery(), ct);
        return Ok(ApiResponse<List<CatalogoItemDto>>.Success(result));
    }

    // ── Inventario ────────────────────────────────────────────────────────────

    /// <summary>Lista los tipos de movimiento de inventario</summary>
    [HttpGet("tipos-movimiento-inventario")]
    public async Task<IActionResult> GetTiposMovInventario(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetTiposMovInventarioQuery(), ct);
        return Ok(ApiResponse<List<CatalogoItemDto>>.Success(result));
    }

    // ── Clientes ─────────────────────────────────────────────────────────────

    /// <summary>Lista los tipos de documento (CC, NIT, Pasaporte…)</summary>
    [HttpGet("tipos-documento")]
    public async Task<IActionResult> GetTiposDocumento(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetTiposDocumentoQuery(), ct);
        return Ok(ApiResponse<List<TipoDocumentoItemDto>>.Success(result));
    }

    // ── Servicios / Órdenes ───────────────────────────────────────────────────

    /// <summary>Lista los tipos de servicio con precio base</summary>
    [HttpGet("tipos-servicio")]
    public async Task<IActionResult> GetTiposServicio(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetTiposServicioQuery(), ct);
        return Ok(ApiResponse<List<TipoServicioItemDto>>.Success(result));
    }

    // ── Facturación ───────────────────────────────────────────────────────────

    /// <summary>Lista los métodos de pago disponibles</summary>
    [HttpGet("metodos-pago")]
    public async Task<IActionResult> GetMetodosPago(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetMetodosPagoQuery(), ct);
        return Ok(ApiResponse<List<CatalogoItemDto>>.Success(result));
    }

    // ── Repuestos ─────────────────────────────────────────────────────────────

    /// <summary>Lista las categorías de repuestos</summary>
    [HttpGet("categorias-repuesto")]
    public async Task<IActionResult> GetCategoriasRepuesto(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetCategoriasRepuestoQuery(), ct);
        return Ok(ApiResponse<List<CategoriaRepuestoItemDto>>.Success(result));
    }

    // ── Seguridad ─────────────────────────────────────────────────────────────

    /// <summary>Lista los permisos del sistema, opcionalmente filtrados por módulo</summary>
    [HttpGet("permisos")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetPermisos([FromQuery] string? modulo, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetPermisosQuery(modulo), ct);
        return Ok(ApiResponse<List<PermisoItemDto>>.Success(result));
    }

    // ── Configuración del Sistema ─────────────────────────────────────────────

    /// <summary>Lista las configuraciones del sistema, opcionalmente filtradas por grupo</summary>
    [HttpGet("configuraciones")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetConfiguraciones([FromQuery] string? grupo, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetConfiguracionesQuery(grupo), ct);
        return Ok(ApiResponse<List<ConfiguracionItemDto>>.Success(result));
    }

    /// <summary>Actualiza el valor de una configuración del sistema</summary>
    [HttpPut("configuraciones/{clave}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<ConfiguracionItemDto>), 200)]
    public async Task<IActionResult> UpdateConfiguracion(string clave, [FromBody] UpdateConfiguracionDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new UpdateConfiguracionCommand(clave, dto.Valor), ct);
        return Ok(ApiResponse<ConfiguracionItemDto>.Success(result));
    }
}
