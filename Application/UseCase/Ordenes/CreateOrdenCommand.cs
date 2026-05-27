using MediatR;
using AutoMapper;
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
        var orden = _mapper.Map<OrdenServicio>(request.Dto);
        orden.NumeroOrden = $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}";
        orden.Estado = EstadoOrdenEnum.Pendiente;
        orden.FechaIngreso = DateTime.UtcNow;
        _context.OrdenesServicio.Add(orden);

        var historial = new HistorialEstadoOrden
        {
            OrdenServicioId = orden.Id,
            Estado = EstadoOrdenEnum.Pendiente,
            FechaHora = DateTime.UtcNow
        };
        _context.HistorialEstadosOrden.Add(historial);

        await _context.SaveChangesAsync(cancellationToken);
        return _mapper.Map<OrdenServicioDto>(orden);
    }
}
