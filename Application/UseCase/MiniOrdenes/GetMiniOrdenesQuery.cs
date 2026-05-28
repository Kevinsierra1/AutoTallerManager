using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Abstractions;
using Application.Common;
using Application.Extensions;

namespace Application.UseCase.MiniOrdenes;

public record GetMiniOrdenesQuery(MiniOrdenFiltroDto Filtro) : IRequest<PagedResult<MiniOrdenDto>>;

public class GetMiniOrdenesQueryHandler : IRequestHandler<GetMiniOrdenesQuery, PagedResult<MiniOrdenDto>>
{
    private readonly IApplicationDbContext _context;
    public GetMiniOrdenesQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<PagedResult<MiniOrdenDto>> Handle(GetMiniOrdenesQuery request, CancellationToken cancellationToken)
    {
        var f = request.Filtro;
        var q = _context.MiniOrdenes
            .Include(m => m.Cliente)
            .Include(m => m.Vehiculo).ThenInclude(v => v.ModeloVehiculo).ThenInclude(mv => mv.Marca)
            .Include(m => m.OrdenServicio)
            .Include(m => m.OrdenArea).ThenInclude(a => a!.AreaTaller)
            .Include(m => m.Mecanico)
            .Include(m => m.JefeTaller)
            .AsQueryable();

        if (f.ClienteId.HasValue)       q = q.Where(m => m.ClienteId == f.ClienteId.Value);
        if (f.VehiculoId.HasValue)      q = q.Where(m => m.VehiculoId == f.VehiculoId.Value);
        if (f.OrdenServicioId.HasValue) q = q.Where(m => m.OrdenServicioId == f.OrdenServicioId.Value);
        if (f.Estado.HasValue)          q = q.Where(m => m.Estado == f.Estado.Value);
        if (f.MecanicoId.HasValue)      q = q.Where(m => m.MecanicoId == f.MecanicoId.Value);

        var projected = q.OrderByDescending(m => m.CreadoEn)
            .Select(m => new MiniOrdenDto(
                m.Id, m.NumeroMiniOrden,
                m.ClienteId, $"{m.Cliente.Nombres} {m.Cliente.Apellidos}",
                m.VehiculoId, m.Vehiculo.Placa,
                $"{m.Vehiculo.ModeloVehiculo.Marca.Nombre} {m.Vehiculo.ModeloVehiculo.Nombre} {m.Vehiculo.Anio}",
                m.OrdenServicioId, m.OrdenServicio != null ? m.OrdenServicio.NumeroOrden : null,
                m.OrdenAreaId, m.OrdenArea != null ? m.OrdenArea.AreaTaller.Nombre : null,
                m.Descripcion, m.Estado, m.Estado.ToString(),
                m.MecanicoId, m.Mecanico != null ? $"{m.Mecanico.Nombres} {m.Mecanico.Apellidos}" : null,
                m.JefeTallerId, m.JefeTaller != null ? $"{m.JefeTaller.Nombres} {m.JefeTaller.Apellidos}" : null,
                m.FechaAprobacionJefe, m.FechaAprobacionCliente,
                m.FechaInicio, m.FechaFin,
                m.TotalMateriales, m.TotalManoObra, m.Total,
                m.Observaciones, m.MotivoRechazo, m.CreadoEn,
                null, null
            ));

        return await projected.ToPagedResultAsync(f.PageNumber, f.PageSize, cancellationToken);
    }
}
