using MediatR;
using AutoMapper;
using Application.Abstractions;
using Domain.Entities;

namespace Application.UseCase.Vehiculos;

public record CreateVehiculoCommand(CreateVehiculoDto Dto) : IRequest<VehiculoDto>;

public class CreateVehiculoCommandHandler : IRequestHandler<CreateVehiculoCommand, VehiculoDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public CreateVehiculoCommandHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<VehiculoDto> Handle(CreateVehiculoCommand request, CancellationToken cancellationToken)
    {
        var vehiculo = _mapper.Map<Vehiculo>(request.Dto);
        vehiculo.Activo = true;
        _context.Vehiculos.Add(vehiculo);
        await _context.SaveChangesAsync(cancellationToken);
        return _mapper.Map<VehiculoDto>(vehiculo);
    }
}
