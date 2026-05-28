using MediatR;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Application.Abstractions;
using Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCase.Vehiculos;

public record GetVehiculoByIdQuery(Guid Id) : IRequest<VehiculoDto>;

public class GetVehiculoByIdQueryHandler : IRequestHandler<GetVehiculoByIdQuery, VehiculoDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    public GetVehiculoByIdQueryHandler(IApplicationDbContext context, IMapper mapper) { _context = context; _mapper = mapper; }

    public async Task<VehiculoDto> Handle(GetVehiculoByIdQuery request, CancellationToken cancellationToken)
    {
        var v = await _context.Vehiculos
            .Include(v => v.ModeloVehiculo).ThenInclude(m => m.Marca)
            .Include(v => v.Color)
            .FirstOrDefaultAsync(v => v.Id == request.Id && !v.Eliminado, cancellationToken)
            ?? throw new NotFoundException("Vehiculo", request.Id);
        return _mapper.Map<VehiculoDto>(v);
    }
}
