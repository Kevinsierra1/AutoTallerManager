using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Abstractions;
using Application.Common;
using Application.Extensions;

namespace Application.UseCase.Notificaciones;

public record GetNotificacionesQuery(Guid UsuarioId, bool? SoloNoLeidas = null, int PageNumber = 1, int PageSize = 20)
    : IRequest<PagedResult<NotificacionDto>>;

public class GetNotificacionesQueryHandler : IRequestHandler<GetNotificacionesQuery, PagedResult<NotificacionDto>>
{
    private readonly IApplicationDbContext _context;
    public GetNotificacionesQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<PagedResult<NotificacionDto>> Handle(GetNotificacionesQuery request, CancellationToken cancellationToken)
    {
        var q = _context.Notificaciones
            .Where(n => n.UsuarioId == request.UsuarioId && !n.Eliminado)
            .AsQueryable();

        if (request.SoloNoLeidas == true)
            q = q.Where(n => !n.Leida);

        var projected = q.OrderByDescending(n => n.FechaCreacion)
            .Select(n => new NotificacionDto(n.Id, n.Titulo, n.Mensaje, n.Tipo, n.Leida, n.FechaCreacion, n.FechaLectura, n.Url));

        return await projected.ToPagedResultAsync(request.PageNumber, request.PageSize, cancellationToken);
    }
}
