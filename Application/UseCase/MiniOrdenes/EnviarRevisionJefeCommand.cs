using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Abstractions;
using Application.Common.Exceptions;
using Domain.Entities;
using Domain.Enums;

namespace Application.UseCase.MiniOrdenes;

public record EnviarRevisionJefeCommand(Guid MiniOrdenId) : IRequest<MiniOrdenDto>;

public class EnviarRevisionJefeCommandHandler : IRequestHandler<EnviarRevisionJefeCommand, MiniOrdenDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    public EnviarRevisionJefeCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    { _context = context; _currentUser = currentUser; }

    public async Task<MiniOrdenDto> Handle(EnviarRevisionJefeCommand request, CancellationToken cancellationToken)
    {
        var m = await _context.MiniOrdenes.FindAsync([request.MiniOrdenId], cancellationToken)
            ?? throw new NotFoundException("MiniOrden", request.MiniOrdenId);

        if (m.Estado != EstadoMiniOrden.Borrador)
            throw new InvalidOperationException("Solo se pueden enviar mini-órdenes en estado Borrador.");

        var estadoAnterior = m.Estado;
        m.Estado = EstadoMiniOrden.EnRevisionJefe;
        m.ActualizadoEn = DateTime.UtcNow;

        _context.MiniOrdenHistoriales.Add(new MiniOrdenHistorial
        {
            Id = Guid.NewGuid(), MiniOrdenId = m.Id,
            EstadoAnterior = estadoAnterior, EstadoNuevo = EstadoMiniOrden.EnRevisionJefe,
            Observacion = "Enviado a revisión del Jefe de Taller",
            NivelAprobacion = NivelAprobacionMJC.Mecanico,
            Fecha = DateTime.UtcNow, CreadoEn = DateTime.UtcNow
        });

        await _context.SaveChangesAsync(cancellationToken);
        return await new GetMiniOrdenByIdQueryHandler(_context).Handle(new GetMiniOrdenByIdQuery(m.Id), cancellationToken);
    }
}
