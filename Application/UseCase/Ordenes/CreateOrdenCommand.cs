using MediatR;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Application.Abstractions;
using Domain.Entities;
using Domain.Enums;

namespace Application.UseCase.Ordenes;

public record CreateOrdenCommand(CreateOrdenDto Dto) : IRequest<OrdenServicioDto>;

public class CreateOrdenCommandHandler : IRequestHandler<CreateOrdenCommand, OrdenServicioDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CreateOrdenCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<OrdenServicioDto> Handle(CreateOrdenCommand request, CancellationToken cancellationToken)
    {
        var orden = new OrdenServicio
        {
            Id = Guid.NewGuid(),
            CreadoEn = DateTime.UtcNow,
            NumeroOrden = $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}",
            ClienteId = request.Dto.ClienteId,
            VehiculoId = request.Dto.VehiculoId,
            Descripcion = request.Dto.Descripcion,
            TipoServicioId = request.Dto.TipoServicioId,
            Estado = EstadoOrdenEnum.Pendiente,
            FechaIngreso = DateTime.UtcNow
        };

        // Detalles (repuestos)
        decimal totalDetalles = 0;
        if (request.Dto.Detalles?.Count > 0)
        {
            orden.DetallesOrdenServicio = new List<DetalleOrdenServicio>();
            foreach (var d in request.Dto.Detalles)
            {
                var detalle = new DetalleOrdenServicio
                {
                    Id = Guid.NewGuid(),
                    OrdenServicioId = orden.Id,
                    RepuestoId = d.RepuestoId,
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario,
                    CreadoEn = DateTime.UtcNow
                };
                orden.DetallesOrdenServicio.Add(detalle);
                totalDetalles += d.Cantidad * d.PrecioUnitario;
            }
        }

        // Mano de obra
        decimal totalManosObra = 0;
        if (request.Dto.ManosObra?.Count > 0)
        {
            orden.ManosObra = new List<ManoObra>();
            foreach (var m in request.Dto.ManosObra)
            {
                var mano = new ManoObra
                {
                    Id = Guid.NewGuid(),
                    OrdenServicioId = orden.Id,
                    Descripcion = m.Descripcion,
                    HorasTrabajadas = m.HorasTrabajadas,
                    Costo = m.Costo,
                    EmpleadoId = m.EmpleadoId,
                    CreadoEn = DateTime.UtcNow
                };
                orden.ManosObra.Add(mano);
                totalManosObra += m.Costo;
            }
        }

        if (totalDetalles > 0 || totalManosObra > 0)
            orden.Total = totalDetalles + totalManosObra;

        _context.OrdenesServicio.Add(orden);

        _context.HistorialEstadosOrden.Add(new HistorialEstadoOrden
        {
            OrdenServicioId = orden.Id,
            Estado = EstadoOrdenEnum.Pendiente,
            FechaHora = DateTime.UtcNow
        });

        await _context.SaveChangesAsync(cancellationToken);

        var guardada = await _context.OrdenesServicio
            .Include(o => o.Cliente)
            .Include(o => o.Vehiculo)
            .FirstAsync(o => o.Id == orden.Id, cancellationToken);
        return _mapper.Map<OrdenServicioDto>(guardada);
    }
}
