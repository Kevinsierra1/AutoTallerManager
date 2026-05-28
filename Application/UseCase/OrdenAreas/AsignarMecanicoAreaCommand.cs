using MediatR;
using Application.Abstractions;
using Application.Common.Exceptions;

namespace Application.UseCase.OrdenAreas;

public record AsignarMecanicoAreaCommand(Guid OrdenAreaId, Guid MecanicoId) : IRequest<OrdenAreaDetalleDto>;

public class AsignarMecanicoAreaCommandHandler : IRequestHandler<AsignarMecanicoAreaCommand, OrdenAreaDetalleDto>
{
    private readonly IApplicationDbContext _context;
    public AsignarMecanicoAreaCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<OrdenAreaDetalleDto> Handle(AsignarMecanicoAreaCommand request, CancellationToken cancellationToken)
    {
        var area = await _context.OrdenAreas.FindAsync([request.OrdenAreaId], cancellationToken)
            ?? throw new NotFoundException("OrdenArea", request.OrdenAreaId);

        _ = await _context.Empleados.FindAsync([request.MecanicoId], cancellationToken)
            ?? throw new NotFoundException("Empleado", request.MecanicoId);

        area.MecanicoId = request.MecanicoId;
        area.ActualizadoEn = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);

        return await new GetOrdenAreaByIdQueryHandler(_context)
            .Handle(new GetOrdenAreaByIdQuery(area.Id), cancellationToken);
    }
}
