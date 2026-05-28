using MediatR;
using Application.Abstractions;
using Application.Common.Exceptions;
using Domain.Enums;

namespace Application.UseCase.OrdenAreas;

public record UpdateEstadoAreaCommand(Guid OrdenAreaId, UpdateEstadoAreaDto Dto) : IRequest<OrdenAreaDetalleDto>;

public class UpdateEstadoAreaCommandHandler : IRequestHandler<UpdateEstadoAreaCommand, OrdenAreaDetalleDto>
{
    private readonly IApplicationDbContext _context;
    public UpdateEstadoAreaCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<OrdenAreaDetalleDto> Handle(UpdateEstadoAreaCommand request, CancellationToken cancellationToken)
    {
        var area = await _context.OrdenAreas.FindAsync([request.OrdenAreaId], cancellationToken)
            ?? throw new NotFoundException("OrdenArea", request.OrdenAreaId);

        area.Estado = request.Dto.NuevoEstado;
        area.Observaciones = request.Dto.Observaciones ?? area.Observaciones;

        if (request.Dto.NuevoEstado == EstadoMiniOrden.EnProceso && area.FechaInicio == null)
            area.FechaInicio = DateTime.UtcNow;

        if (request.Dto.NuevoEstado == EstadoMiniOrden.Completada)
            area.FechaFin = DateTime.UtcNow;

        area.ActualizadoEn = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return await new GetOrdenAreaByIdQueryHandler(_context)
            .Handle(new GetOrdenAreaByIdQuery(area.Id), cancellationToken);
    }
}
