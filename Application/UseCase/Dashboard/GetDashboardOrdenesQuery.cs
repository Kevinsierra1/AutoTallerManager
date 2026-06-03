using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Abstractions;
using Domain.Enums;

namespace Application.UseCase.Dashboard;

public record OrdenEstadisticaDto(string Estado, int Cantidad);

public record GetDashboardOrdenesQuery : IRequest<List<OrdenEstadisticaDto>>;

public class GetDashboardOrdenesQueryHandler : IRequestHandler<GetDashboardOrdenesQuery, List<OrdenEstadisticaDto>>
{
    private readonly IApplicationDbContext _context;
    public GetDashboardOrdenesQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<List<OrdenEstadisticaDto>> Handle(GetDashboardOrdenesQuery request, CancellationToken cancellationToken)
    {
        // Enum.ToString() no es traducible a SQL — traer a memoria y convertir
        var raw = await _context.OrdenesServicio
            .GroupBy(o => o.Estado)
            .Select(g => new { Estado = (int)g.Key, Cantidad = g.Count() })
            .ToListAsync(cancellationToken);
        return raw.Select(x => new OrdenEstadisticaDto(((EstadoOrdenEnum)x.Estado).ToString(), x.Cantidad)).ToList();
    }
}
