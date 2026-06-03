using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Abstractions;
using Application.Common.Exceptions;

namespace Application.UseCase.MiniOrdenes;

public record GetMiniOrdenByIdQuery(Guid Id) : IRequest<MiniOrdenDto>;

public class GetMiniOrdenByIdQueryHandler : IRequestHandler<GetMiniOrdenByIdQuery, MiniOrdenDto>
{
    private readonly IApplicationDbContext _context;
    public GetMiniOrdenByIdQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<MiniOrdenDto> Handle(GetMiniOrdenByIdQuery request, CancellationToken cancellationToken)
    {
        var m = await _context.MiniOrdenes
            .Include(x => x.Cliente)
            .Include(x => x.Vehiculo).ThenInclude(v => v.ModeloVehiculo).ThenInclude(mv => mv.Marca)
            .Include(x => x.OrdenServicio)
            .Include(x => x.OrdenArea).ThenInclude(a => a!.AreaTaller)
            .Include(x => x.Mecanico)
            .Include(x => x.JefeTaller)
            .Include(x => x.Detalles!).ThenInclude(d => d.Repuesto)
            .Include(x => x.ManosObra!).ThenInclude(mo => mo.Tecnico)
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.Eliminado, cancellationToken)
            ?? throw new NotFoundException("Presupuesto", request.Id);

        return MapToDto(m);
    }

    internal static MiniOrdenDto MapToDto(Domain.Entities.MiniOrden m) => new(
        m.Id,
        m.NumeroMiniOrden,
        m.ClienteId,
        $"{m.Cliente.Nombres} {m.Cliente.Apellidos}",
        m.VehiculoId,
        m.Vehiculo.Placa,
        $"{m.Vehiculo.ModeloVehiculo?.Marca?.Nombre} {m.Vehiculo.ModeloVehiculo?.Nombre} {m.Vehiculo.Anio}".Trim(),
        m.OrdenServicioId,
        m.OrdenServicio?.NumeroOrden,
        m.OrdenAreaId,
        m.OrdenArea?.AreaTaller.Nombre,
        m.Descripcion,
        m.Estado,
        m.Estado.ToString(),
        m.MecanicoId,
        m.Mecanico != null ? $"{m.Mecanico.Nombres} {m.Mecanico.Apellidos}" : null,
        m.JefeTallerId,
        m.JefeTaller != null ? $"{m.JefeTaller.Nombres} {m.JefeTaller.Apellidos}" : null,
        m.FechaAprobacionJefe,
        m.FechaAprobacionCliente,
        m.FechaInicio,
        m.FechaFin,
        m.TotalMateriales,
        m.TotalManoObra,
        m.Total,
        m.Observaciones,
        m.MotivoRechazo,
        m.CreadoEn,
        m.Detalles?.Select(d => new MiniOrdenDetalleDto(
            d.Id, d.RepuestoId, d.Repuesto.Nombre, d.Repuesto.Codigo,
            d.Cantidad, d.PrecioUnitario, d.Subtotal)).ToList(),
        m.ManosObra?.Select(mo => new MiniOrdenManoObraDto(
            mo.Id, mo.Descripcion, mo.HorasTrabajo, mo.TarifaHora, mo.Total,
            mo.TecnicoId, mo.Tecnico != null ? $"{mo.Tecnico.Nombres} {mo.Tecnico.Apellidos}" : null)).ToList()
    );
}
