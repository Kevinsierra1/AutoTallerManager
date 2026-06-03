using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Abstractions;
using Application.Common.Exceptions;
using Domain.Entities;

namespace Application.UseCase.MiniOrdenes;

public record AddManoObraMiniOrdenCommand(Guid MiniOrdenId, CreateMiniOrdenManoObraDto Dto) : IRequest<MiniOrdenManoObraDto>;

public class AddManoObraMiniOrdenCommandHandler : IRequestHandler<AddManoObraMiniOrdenCommand, MiniOrdenManoObraDto>
{
    private readonly IApplicationDbContext _context;
    public AddManoObraMiniOrdenCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<MiniOrdenManoObraDto> Handle(AddManoObraMiniOrdenCommand request, CancellationToken cancellationToken)
    {
        var mini = await _context.MiniOrdenes
            .Include(m => m.ManosObra)
            .FirstOrDefaultAsync(m => m.Id == request.MiniOrdenId, cancellationToken)
            ?? throw new NotFoundException("Presupuesto", request.MiniOrdenId);

        Empleado? tecnico = null;
        if (request.Dto.TecnicoId.HasValue)
            tecnico = await _context.Empleados.FirstOrDefaultAsync(e => e.Id == request.Dto.TecnicoId.Value, cancellationToken);

        var total = request.Dto.HorasTrabajo * request.Dto.TarifaHora;
        var mano = new MiniOrdenManoObra
        {
            Id = Guid.NewGuid(),
            MiniOrdenId = mini.Id,
            Descripcion = request.Dto.Descripcion,
            HorasTrabajo = request.Dto.HorasTrabajo,
            TarifaHora = request.Dto.TarifaHora,
            Total = total,
            TecnicoId = request.Dto.TecnicoId,
            CreadoEn = DateTime.UtcNow
        };

        // Capturar la suma ANTES de Add para evitar el doble conteo por EF relationship fixup
        var existingMO = (mini.ManosObra ?? []).Sum(m => m.Total);
        _context.MiniOrdenManosObra.Add(mano);

        var totalMO = existingMO + total;
        mini.TotalManoObra = totalMO;
        mini.Total = mini.TotalMateriales + totalMO;

        await _context.SaveChangesAsync(cancellationToken);

        var nombreTec = tecnico != null ? $"{tecnico.Nombres} {tecnico.Apellidos}" : null;
        return new MiniOrdenManoObraDto(mano.Id, mano.Descripcion, mano.HorasTrabajo,
            mano.TarifaHora, mano.Total, mano.TecnicoId, nombreTec);
    }
}
