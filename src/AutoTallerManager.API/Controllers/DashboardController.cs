using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using AutoTallerManager.Application.Features.Dashboard.Queries;
using AutoTallerManager.Application.Features.Dashboard.DTOs;
using AutoTallerManager.Shared.Models;

namespace AutoTallerManager.API.Controllers;

/// <summary>Dashboard y Reportes</summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IMediator _mediator;
    public DashboardController(IMediator mediator) => _mediator = mediator;

    /// <summary>Obtiene resumen ejecutivo del taller</summary>
    [HttpGet("resumen")]
    [ProducesResponseType(typeof(ApiResponse<DashboardResumenDto>), 200)]
    public async Task<IActionResult> GetResumen(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetDashboardResumenQuery(), ct);
        return Ok(ApiResponse<DashboardResumenDto>.Success(result));
    }

    /// <summary>Obtiene órdenes por estado</summary>
    [HttpGet("ordenes-por-estado")]
    [ProducesResponseType(typeof(ApiResponse<List<OrdenEstadisticaDto>>), 200)]
    public async Task<IActionResult> GetOrdenesPorEstado(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetDashboardOrdenesQuery(), ct);
        return Ok(ApiResponse<List<OrdenEstadisticaDto>>.Success(result));
    }

    /// <summary>Obtiene facturación mensual (últimos 6 meses)</summary>
    [HttpGet("facturacion-mensual")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(ApiResponse<List<FacturacionMensualDto>>), 200)]
    public async Task<IActionResult> GetFacturacionMensual(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetDashboardFacturacionQuery(), ct);
        return Ok(ApiResponse<List<FacturacionMensualDto>>.Success(result));
    }

    /// <summary>Obtiene repuestos con stock crítico</summary>
    [HttpGet("repuestos-criticos")]
    [Authorize(Roles = "Admin,Recepcionista")]
    [ProducesResponseType(200)]
    public async Task<IActionResult> GetRepuestosCriticos(int pageNumber = 1, int pageSize = 10, CancellationToken ct = default)
    {
        var result = await _mediator.Send(new GetRepuestosCriticosQuery(pageNumber, pageSize), ct);
        return Ok(ApiResponse<object>.Success(result));
    }
}
