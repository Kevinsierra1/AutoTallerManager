using MediatR;
using Application.Abstractions;
using Application.Common.Exceptions;
using Domain.Entities;
using Domain.Enums;

namespace Application.UseCase.OrdenAreas;

public record CreateOrdenAreaCommand(CreateOrdenAreaDto Dto) : IRequest<OrdenAreaDetalleDto>;

public class CreateOrdenAreaCommandHandler : IRequestHandler<CreateOrdenAreaCommand, OrdenAreaDetalleDto>
{
    private readonly IApplicationDbContext _context;
    public CreateOrdenAreaCommandHandler(IApplicationDbContext context) => _context = context;

    public async Task<OrdenAreaDetalleDto> Handle(CreateOrdenAreaCommand request, CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        _ = await _context.OrdenesServicio.FindAsync([dto.OrdenServicioId], cancellationToken)
            ?? throw new NotFoundException("OrdenServicio", dto.OrdenServicioId);

        _ = await _context.AreasTaller.FindAsync([dto.AreaTallerId], cancellationToken)
            ?? throw new NotFoundException("AreaTaller", dto.AreaTallerId);

        var ordenArea = new OrdenArea
        {
            Id = Guid.NewGuid(),
            OrdenServicioId = dto.OrdenServicioId,
            AreaTallerId = dto.AreaTallerId,
            MecanicoId = dto.MecanicoId,
            Estado = EstadoMiniOrden.Borrador,
            Observaciones = dto.Observaciones,
            CreadoEn = DateTime.UtcNow
        };

        _context.OrdenAreas.Add(ordenArea);
        await _context.SaveChangesAsync(cancellationToken);

        return await new GetOrdenAreaByIdQueryHandler(_context)
            .Handle(new GetOrdenAreaByIdQuery(ordenArea.Id), cancellationToken);
    }
}
