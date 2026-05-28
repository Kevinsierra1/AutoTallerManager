using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Abstractions;
using Application.Common.Exceptions;

namespace Application.UseCase.Notificaciones;

public record MarcarLeidaCommand(Guid NotificacionId, Guid UsuarioId) : IRequest;

public class MarcarLeidaCommandHandler : IRequestHandler<MarcarLeidaCommand>
{
    private readonly IApplicationDbContext _context;
    public MarcarLeidaCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(MarcarLeidaCommand request, CancellationToken cancellationToken)
    {
        var n = await _context.Notificaciones
            .FirstOrDefaultAsync(n => n.Id == request.NotificacionId && n.UsuarioId == request.UsuarioId, cancellationToken)
            ?? throw new NotFoundException("Notificacion", request.NotificacionId);
        n.Leida = true;
        n.FechaLectura = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
    }
}

public record MarcarTodasLeidasCommand(Guid UsuarioId) : IRequest<int>;

public class MarcarTodasLeidasCommandHandler : IRequestHandler<MarcarTodasLeidasCommand, int>
{
    private readonly IApplicationDbContext _context;
    public MarcarTodasLeidasCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<int> Handle(MarcarTodasLeidasCommand request, CancellationToken cancellationToken)
    {
        var noLeidas = await _context.Notificaciones
            .Where(n => n.UsuarioId == request.UsuarioId && !n.Leida && !n.Eliminado)
            .ToListAsync(cancellationToken);

        foreach (var n in noLeidas) { n.Leida = true; n.FechaLectura = DateTime.UtcNow; }
        await _context.SaveChangesAsync(cancellationToken);
        return noLeidas.Count;
    }
}
