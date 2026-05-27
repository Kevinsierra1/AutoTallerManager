using MediatR;
using Microsoft.EntityFrameworkCore;
using AutoTallerManager.Application.Common.Interfaces;
using AutoTallerManager.Domain.Enums;

namespace AutoTallerManager.Application.Features.Dashboard.Queries;

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
