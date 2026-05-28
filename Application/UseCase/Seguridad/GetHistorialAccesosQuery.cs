using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Abstractions;
using Application.Common;
using Application.Extensions;

namespace Application.UseCase.Seguridad;

public record GetHistorialAccesosQuery(Guid? UsuarioId, bool? SoloFallidos, int PageNumber = 1, int PageSize = 20)
    : IRequest<PagedResult<HistorialAccesoDto>>;

public class GetHistorialAccesosQueryHandler : IRequestHandler<GetHistorialAccesosQuery, PagedResult<HistorialAccesoDto>>
{
    private readonly IApplicationDbContext _context;
    public GetHistorialAccesosQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<PagedResult<HistorialAccesoDto>> Handle(GetHistorialAccesosQuery request, CancellationToken cancellationToken)
    {
        var q = _context.HistorialAccesos.Include(h => h.Usuario).AsQueryable();

        if (request.UsuarioId.HasValue)
            q = q.Where(h => h.UsuarioId == request.UsuarioId.Value);
        if (request.SoloFallidos == true)
            q = q.Where(h => !h.Exitoso);

        var projected = q.OrderByDescending(h => h.FechaAcceso)
            .Select(h => new HistorialAccesoDto(
                h.Id, h.UsuarioId, h.Usuario.Email, h.IpAddress, h.FechaAcceso, h.Exitoso, h.MotivoFallo, h.UserAgent));

        return await projected.ToPagedResultAsync(request.PageNumber, request.PageSize, cancellationToken);
    }
}
