using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Abstractions;
using Application.Common;
using Application.Extensions;

namespace Application.UseCase.Seguridad;

public record GetLogsErroresQuery(string? Busqueda, string? Endpoint, int PageNumber = 1, int PageSize = 20)
    : IRequest<PagedResult<LogErrorDto>>;

public class GetLogsErroresQueryHandler : IRequestHandler<GetLogsErroresQuery, PagedResult<LogErrorDto>>
{
    private readonly IApplicationDbContext _context;
    public GetLogsErroresQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<PagedResult<LogErrorDto>> Handle(GetLogsErroresQuery request, CancellationToken cancellationToken)
    {
        var q = _context.LogsErrores.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Busqueda))
            q = q.Where(l => l.Mensaje.Contains(request.Busqueda));
        if (!string.IsNullOrWhiteSpace(request.Endpoint))
            q = q.Where(l => l.Endpoint != null && l.Endpoint.Contains(request.Endpoint));

        var projected = q.OrderByDescending(l => l.Fecha)
            .Select(l => new LogErrorDto(l.Id, l.Mensaje, l.StackTrace, l.UsuarioId, l.Endpoint, l.Fecha));

        return await projected.ToPagedResultAsync(request.PageNumber, request.PageSize, cancellationToken);
    }
}
