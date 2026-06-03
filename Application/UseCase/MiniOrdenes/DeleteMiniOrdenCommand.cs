using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Abstractions;
using Application.Common.Exceptions;
using Domain.Exceptions;
using Domain.Enums;

namespace Application.UseCase.MiniOrdenes;

public record DeleteMiniOrdenCommand(Guid Id) : IRequest;

public class DeleteMiniOrdenCommandHandler : IRequestHandler<DeleteMiniOrdenCommand>
{
    private readonly IApplicationDbContext _context;
    public DeleteMiniOrdenCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(DeleteMiniOrdenCommand request, CancellationToken cancellationToken)
    {
        var mini = await _context.MiniOrdenes
            .FirstOrDefaultAsync(m => m.Id == request.Id && !m.Eliminado, cancellationToken)
            ?? throw new NotFoundException("Presupuesto", request.Id);

        // No eliminar si ya generó una OS y está en proceso o finalizada
        if (mini.OrdenServicioId.HasValue)
        {
            var os = await _context.OrdenesServicio
                .FirstOrDefaultAsync(o => o.Id == mini.OrdenServicioId && !o.Eliminado, cancellationToken);
            if (os != null && os.Estado == EstadoOrdenEnum.Finalizada)
                throw new DomainException("No se puede eliminar un presupuesto cuya orden de servicio ya fue finalizada.");
        }

        mini.Eliminado = true;
        mini.ActualizadoEn = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
    }
}
