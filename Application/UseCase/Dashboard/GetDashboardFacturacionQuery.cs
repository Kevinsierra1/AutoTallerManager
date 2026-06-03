using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Abstractions;

namespace Application.UseCase.Dashboard;

public record FacturacionMensualDto(string Mes, decimal Total, int CantidadFacturas);

public record GetDashboardFacturacionQuery : IRequest<List<FacturacionMensualDto>>;

public class GetDashboardFacturacionQueryHandler : IRequestHandler<GetDashboardFacturacionQuery, List<FacturacionMensualDto>>
{
    private readonly IApplicationDbContext _context;
    public GetDashboardFacturacionQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<FacturacionMensualDto>> Handle(GetDashboardFacturacionQuery request, CancellationToken cancellationToken)
    {
        var inicio = DateTime.UtcNow.AddMonths(-6);
        // Proyectar a tipo anónimo en SQL, luego mapear en memoria para evitar problemas de traducción
        var raw = await _context.Facturas
            .Where(f => f.CreadoEn >= inicio)
            .GroupBy(f => new { f.CreadoEn.Year, f.CreadoEn.Month })
            .Select(g => new { g.Key.Year, g.Key.Month, Total = g.Sum(f => f.Total), Cantidad = g.Count() })
            .OrderBy(x => x.Year).ThenBy(x => x.Month)
            .ToListAsync(cancellationToken);
        return raw.Select(x => new FacturacionMensualDto($"{x.Year}-{x.Month:D2}", x.Total, x.Cantidad)).ToList();
    }
}
