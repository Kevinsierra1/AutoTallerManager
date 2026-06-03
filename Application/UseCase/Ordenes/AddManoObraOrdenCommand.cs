using MediatR;
using Microsoft.EntityFrameworkCore;
using Application.Abstractions;
using Application.Common.Exceptions;
using Domain.Entities;

namespace Application.UseCase.Ordenes;

public record AddManoObraOrdenCommand(Guid OrdenId, CreateManoObraOrdenDto Dto) : IRequest<ManoObraOrdenDto>;

public class AddManoObraOrdenCommandHandler : IRequestHandler<AddManoObraOrdenCommand, ManoObraOrdenDto>
{
    private readonly IApplicationDbContext _context;

    public AddManoObraOrdenCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<ManoObraOrdenDto> Handle(AddManoObraOrdenCommand request, CancellationToken cancellationToken)
    {
        var orden = await _context.OrdenesServicio
            .Include(o => o.ManosObra)
            .FirstOrDefaultAsync(o => o.Id == request.OrdenId, cancellationToken)
            ?? throw new NotFoundException("OrdenServicio", request.OrdenId);

        Empleado? empleado = null;
        if (request.Dto.EmpleadoId.HasValue)
        {
            empleado = await _context.Empleados
                .FirstOrDefaultAsync(e => e.Id == request.Dto.EmpleadoId.Value, cancellationToken);
        }

        var mano = new ManoObra
        {
            Id = Guid.NewGuid(),
            OrdenServicioId = orden.Id,
            Descripcion = request.Dto.Descripcion,
            HorasTrabajadas = request.Dto.HorasTrabajadas,
            Costo = request.Dto.Costo,
            EmpleadoId = request.Dto.EmpleadoId,
            CreadoEn = DateTime.UtcNow
        };

        // Capturar suma ANTES de Add para evitar doble conteo por EF relationship fixup
        var existingMO = (orden.ManosObra ?? []).Sum(m => m.Costo);
        _context.ManosObra.Add(mano);

        // Recalcular total
        var subtotalDetalles = await _context.DetallesOrdenServicio
            .Where(d => d.OrdenServicioId == orden.Id)
            .SumAsync(d => d.Cantidad * d.PrecioUnitario, cancellationToken);

        var subtotalManosObra = existingMO + mano.Costo;

        orden.Total = subtotalDetalles + subtotalManosObra;

        await _context.SaveChangesAsync(cancellationToken);

        var nombreEmpleado = empleado != null ? $"{empleado.Nombres} {empleado.Apellidos}" : null;
        return new ManoObraOrdenDto(mano.Id, mano.Descripcion, mano.Costo, mano.HorasTrabajadas,
            mano.EmpleadoId, nombreEmpleado);
    }
}
