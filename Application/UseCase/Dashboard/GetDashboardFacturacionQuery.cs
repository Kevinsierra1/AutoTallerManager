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
        var facturas = await _context.Facturas
            .Where(f => f.CreadoEn >= inicio)
            .GroupBy(f => new { f.CreadoEn.Year, f.CreadoEn.Month })
            .Select(g => new FacturacionMensualDto(
                $"{g.Key.Year}-{g.Key.Month:D2}",
                g.Sum(f => f.Total),
                g.Count()
            ))
            .OrderBy(x => x.Mes)
            .ToListAsync(cancellationToken);
        return facturas;
    }
}
