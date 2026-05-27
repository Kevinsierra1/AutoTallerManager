using MediatR;
using AutoTallerManager.Application.Common.Interfaces;
using AutoTallerManager.Application.Common.Exceptions;
using AutoTallerManager.Domain.Enums;

namespace AutoTallerManager.Application.Features.Ordenes.Commands;

public record AsignarMecanicoCommand(Guid OrdenId, Guid EmpleadoId) : IRequest;

public class AsignarMecanicoCommandHandler : IRequestHandler<AsignarMecanicoCommand>
{
    private readonly IApplicationDbContext _context;
    public AsignarMecanicoCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(AsignarMecanicoCommand request, CancellationToken cancellationToken)
    {
        var orden = await _context.OrdenesServicio.FindAsync([request.OrdenId], cancellationToken)
            ?? throw new NotFoundException("OrdenServicio", request.OrdenId);
        _ = await _context.Empleados.FindAsync([request.EmpleadoId], cancellationToken)
            ?? throw new NotFoundException("Empleado", request.EmpleadoId);
        orden.MecanicoId = request.EmpleadoId;
        if (orden.Estado == EstadoOrdenEnum.Pendiente || orden.Estado == EstadoOrdenEnum.Aprobada)
            orden.Estado = EstadoOrdenEnum.EnProceso;
        await _context.SaveChangesAsync(cancellationToken);
    }
}
