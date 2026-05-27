using MediatR;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Application.Abstractions;
using Application.Common.Exceptions;

namespace Application.UseCase.Ordenes;

public record GetOrdenByIdQuery(Guid Id) : IRequest<OrdenServicioDto>;

public class GetOrdenByIdQueryHandler : IRequestHandler<GetOrdenByIdQuery, OrdenServicioDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetOrdenByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<OrdenServicioDto> Handle(GetOrdenByIdQuery request, CancellationToken cancellationToken)
    {
        var orden = await _context.OrdenesServicio
            .Include(o => o.Cliente)
            .Include(o => o.Vehiculo)
            .Include(o => o.Mecanico)
            .FirstOrDefaultAsync(o => o.Id == request.Id, cancellationToken)
            ?? throw new NotFoundException("OrdenServicio", request.Id);
        return _mapper.Map<OrdenServicioDto>(orden);
    }
}
