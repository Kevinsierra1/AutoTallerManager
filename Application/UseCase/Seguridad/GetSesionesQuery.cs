using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Abstractions;
using Application.Common;
using Application.Extensions;

namespace Application.UseCase.Seguridad;

public record GetSesionesQuery(Guid? UsuarioId, bool? SoloActivas, int PageNumber = 1, int PageSize = 20)
    : IRequest<PagedResult<SesionUsuarioDto>>;

public class GetSesionesQueryHandler : IRequestHandler<GetSesionesQuery, PagedResult<SesionUsuarioDto>>
{
    private readonly IApplicationDbContext _context;
    public GetSesionesQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<PagedResult<SesionUsuarioDto>> Handle(GetSesionesQuery request, CancellationToken cancellationToken)
    {
        var q = _context.SesionesUsuarios.Include(s => s.Usuario).AsQueryable();

        if (request.UsuarioId.HasValue)
            q = q.Where(s => s.UsuarioId == request.UsuarioId.Value);
        if (request.SoloActivas == true)
            q = q.Where(s => s.Activa);

        var projected = q.OrderByDescending(s => s.FechaInicio)
            .Select(s => new SesionUsuarioDto(
                s.Id, s.UsuarioId, s.Usuario.Email, s.IpAddress, s.UserAgent, s.FechaInicio, s.FechaFin, s.Activa));

        return await projected.ToPagedResultAsync(request.PageNumber, request.PageSize, cancellationToken);
    }
}
