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
        return await _context.OrdenesServicio
            .GroupBy(o => o.Estado)
            .Select(g => new OrdenEstadisticaDto(g.Key.ToString(), g.Count()))
            .ToListAsync(cancellationToken);
    }
}
