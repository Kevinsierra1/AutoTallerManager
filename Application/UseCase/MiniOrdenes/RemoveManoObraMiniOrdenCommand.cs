using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Abstractions;
using Application.Common.Exceptions;

namespace Application.UseCase.MiniOrdenes;

public record RemoveManoObraMiniOrdenCommand(Guid MiniOrdenId, Guid ManoObraId) : IRequest;

public class RemoveManoObraMiniOrdenCommandHandler : IRequestHandler<RemoveManoObraMiniOrdenCommand>
{
    private readonly IApplicationDbContext _context;
    public RemoveManoObraMiniOrdenCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task Handle(RemoveManoObraMiniOrdenCommand request, CancellationToken cancellationToken)
    {
        var mano = await _context.MiniOrdenManosObra
            .FirstOrDefaultAsync(m => m.Id == request.ManoObraId && m.MiniOrdenId == request.MiniOrdenId, cancellationToken)
            ?? throw new NotFoundException("MiniOrdenManoObra", request.ManoObraId);

        _context.MiniOrdenManosObra.Remove(mano);

        var mini = await _context.MiniOrdenes.FirstOrDefaultAsync(m => m.Id == request.MiniOrdenId, cancellationToken);
        if (mini != null)
        {
            var totalMO = await _context.MiniOrdenManosObra
                .Where(m => m.MiniOrdenId == request.MiniOrdenId && m.Id != request.ManoObraId)
                .SumAsync(m => m.Total, cancellationToken);
            mini.TotalManoObra = totalMO;
            mini.Total = mini.TotalMateriales + totalMO;
        }

        await _context.SaveChangesAsync(cancellationToken);
    }
}
