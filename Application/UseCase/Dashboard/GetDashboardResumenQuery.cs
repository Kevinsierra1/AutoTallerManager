using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Abstractions;
using Domain.Enums;

namespace Application.UseCase.Dashboard;

public record GetDashboardResumenQuery : IRequest<DashboardResumenDto>;

public class GetDashboardResumenQueryHandler : IRequestHandler<GetDashboardResumenQuery, DashboardResumenDto>
{
    private readonly IApplicationDbContext _context;
    public GetDashboardResumenQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<DashboardResumenDto> Handle(GetDashboardResumenQuery request, CancellationToken cancellationToken)
    {
        var inicio = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);

        var totalClientes = await _context.Clientes.CountAsync(cancellationToken);
        var totalVehiculos = await _context.Vehiculos.CountAsync(cancellationToken);
        var ordenesActivas = await _context.OrdenesServicio.CountAsync(o =>
            o.Estado == EstadoOrdenEnum.Pendiente || o.Estado == EstadoOrdenEnum.EnProceso || o.Estado == EstadoOrdenEnum.Aprobada, cancellationToken);
        var ordenesFinalizadas = await _context.OrdenesServicio.CountAsync(o => o.Estado == EstadoOrdenEnum.Finalizada, cancellationToken);
        var repuestosCriticos = await _context.Repuestos.CountAsync(r => r.StockActual <= r.StockMinimo, cancellationToken);
        var facturacionMensual = await _context.Facturas.Where(f => f.FechaEmision >= inicio).SumAsync(f => f.Total, cancellationToken);

        return new DashboardResumenDto(totalClientes, totalVehiculos, ordenesActivas, ordenesFinalizadas, repuestosCriticos, facturacionMensual);
    }
}
