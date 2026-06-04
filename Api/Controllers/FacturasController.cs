using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Application.Abstractions;
using Application.UseCase.Facturas;
using Application.Common;

namespace Api.Controllers;

/// <summary>Gestión de Facturación</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FacturasController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IApplicationDbContext _context;
    public FacturasController(IMediator mediator, IApplicationDbContext context)
    {
        _mediator = mediator;
        _context  = context;
    }

    /// <summary>Obtiene facturas paginadas</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<FacturaDto>>), 200)]
    public async Task<IActionResult> GetAll(int pageNumber = 1, int pageSize = 10, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetFacturasQuery(pageNumber, pageSize), ct);
        Response.Headers["X-Total-Count"] = result.TotalCount.ToString();
        return Ok(ApiResponse<PagedResult<FacturaDto>>.Success(result));
    }

    /// <summary>Obtiene una factura por ID</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ApiResponse<FacturaDto>), 200)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetFacturaByIdQuery(id), ct);
        return Ok(ApiResponse<FacturaDto>.Success(result));
    }

    /// <summary>Genera una factura CONSOLIDADA con todas las órdenes finalizadas de un cliente</summary>
    [HttpPost("consolidada")]
    [Authorize(Roles = "Admin,Recepcionista")]
    [ProducesResponseType(typeof(ApiResponse<FacturaDto>), 201)]
    public async Task<IActionResult> Consolidada([FromBody] GenerarFacturaConsolidadaDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new GenerarFacturaConsolidadaCommand(dto), ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<FacturaDto>.Success(result));
    }

    /// <summary>Lista órdenes finalizadas sin facturar de un cliente</summary>
    [HttpGet("ordenes-pendientes/{clienteId:guid}")]
    [Authorize(Roles = "Admin,Recepcionista")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> OrdenesPendientes(Guid clienteId, CancellationToken ct)
    {
        // Cargar órdenes con sus detalles para calcular el total real
        var ordenes = await _context.OrdenesServicio
            .Include(o => o.DetallesOrdenServicio)
            .Include(o => o.ManosObra)
            .Where(o => o.ClienteId == clienteId
                     && o.Estado == Domain.Enums.EstadoOrdenEnum.Finalizada
                     && o.FacturaId == null
                     && !o.Eliminado)
            .OrderBy(o => o.FechaIngreso)
            .ToListAsync(ct);

        var resultado = ordenes.Select(o => {
            // Calcular subtotal desde detalles + mano de obra
            var subRep = o.DetallesOrdenServicio?.Sum(d => d.Cantidad * d.PrecioUnitario) ?? 0;
            var subMO  = o.ManosObra?.Sum(m => m.Costo) ?? 0;
            var calculado = subRep + subMO;
            // Usar o.Total si no hay detalles desagregados (órdenes heredadas de presupuesto)
            var total = calculado > 0 ? calculado : (o.Total ?? 0);
            return new {
                o.Id,
                o.NumeroOrden,
                o.Descripcion,
                Total       = total,
                SubRepuestos= subRep,
                SubManoObra = subMO,
                o.FechaIngreso,
                o.FechaFin
            };
        }).ToList();

        return Ok(ApiResponse<object>.Success(resultado));
    }

    /// <summary>Genera una factura para una orden finalizada</summary>
    [HttpPost("generar")]
    [Authorize(Roles = "Admin,Recepcionista")]
    [ProducesResponseType(typeof(ApiResponse<FacturaDto>), 201)]
    public async Task<IActionResult> Generar([FromBody] GenerarFacturaDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new GenerarFacturaCommand(dto), ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<FacturaDto>.Success(result));
    }

    /// <summary>Registra el pago de una factura pendiente (interno — admin/recep)</summary>
    [HttpPost("{id:guid}/pagar")]
    [Authorize(Roles = "Admin,Recepcionista")]
    [ProducesResponseType(typeof(ApiResponse<FacturaDto>), 200)]
    public async Task<IActionResult> Pagar(Guid id, [FromBody] RegistrarPagoDto dto, CancellationToken ct)
    {
        var result = await _mediator.Send(new RegistrarPagoCommand(id, dto), ct);
        return Ok(ApiResponse<FacturaDto>.Success(result));
    }

    /// <summary>Cliente: ver mis facturas</summary>
    [HttpGet("mis-facturas")]
    [Authorize(Roles = "Cliente,Admin,Recepcionista")]
    public async Task<IActionResult> MisFacturas(int pageNumber = 1, int pageSize = 20, CancellationToken ct = default)
    {
        var clienteId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? Guid.Empty.ToString());
        // Para admin/recep que prueban como cliente, usar el ClienteId del token si existe
        var result = await _mediator.Send(new GetMisFacturasQuery(clienteId, pageNumber, pageSize), ct);
        Response.Headers["X-Total-Count"] = result.TotalCount.ToString();
        return Ok(ApiResponse<PagedResult<FacturaDto>>.Success(result));
    }

    /// <summary>Cliente: iniciar proceso de pago (genera token para electrónicos)</summary>
    [HttpPost("iniciar-pago")]
    [Authorize(Roles = "Cliente,Admin,Recepcionista")]
    [ProducesResponseType(typeof(ApiResponse<SolicitudPagoDto>), 201)]
    public async Task<IActionResult> IniciarPago([FromBody] IniciarPagoDto dto, CancellationToken ct)
    {
        var clienteId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? Guid.Empty.ToString());
        var result = await _mediator.Send(new IniciarPagoCommand(clienteId, dto), ct);
        return CreatedAtAction(nameof(GetById), new { id = result.FacturaId }, ApiResponse<SolicitudPagoDto>.Success(result));
    }

    /// <summary>Admin/Recep: ver solicitudes de pago (para confirmar efectivo y ver tokens)</summary>
    [HttpGet("solicitudes-pago")]
    [Authorize(Roles = "Admin,Recepcionista")]
    [ProducesResponseType(typeof(ApiResponse<PagedResult<SolicitudPagoDto>>), 200)]
    public async Task<IActionResult> GetSolicitudes([FromQuery] string? estado, int pageNumber = 1, int pageSize = 20, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetSolicitudesPagoQuery(estado, pageNumber, pageSize), ct);
        Response.Headers["X-Total-Count"] = result.TotalCount.ToString();
        return Ok(ApiResponse<PagedResult<SolicitudPagoDto>>.Success(result));
    }

    /// <summary>Admin/Recep: confirmar que el cliente pagó en efectivo</summary>
    [HttpPost("solicitudes-pago/{solicitudId:guid}/confirmar")]
    [Authorize(Roles = "Admin,Recepcionista")]
    [ProducesResponseType(typeof(ApiResponse<SolicitudPagoDto>), 200)]
    public async Task<IActionResult> ConfirmarEfectivo(Guid solicitudId, [FromBody] ConfirmarPagoDto dto, CancellationToken ct)
    {
        var nombre = $"{User.FindFirstValue(ClaimTypes.GivenName)} {User.FindFirstValue(ClaimTypes.Surname)}".Trim();
        var result = await _mediator.Send(new ConfirmarPagoEfectivoCommand(solicitudId, nombre, dto.Observaciones), ct);
        return Ok(ApiResponse<SolicitudPagoDto>.Success(result));
    }
}
