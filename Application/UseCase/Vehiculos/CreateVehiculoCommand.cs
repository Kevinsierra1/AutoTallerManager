using MediatR;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
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
        vehiculo.Id = Guid.NewGuid();
        vehiculo.CreadoEn = DateTime.UtcNow;
        vehiculo.Activo = true;
        _context.Vehiculos.Add(vehiculo);
        await _context.SaveChangesAsync(cancellationToken);

        var guardado = await _context.Vehiculos
            .Include(v => v.ModeloVehiculo).ThenInclude(m => m.Marca)
            .Include(v => v.Color)
            .FirstAsync(v => v.Id == vehiculo.Id, cancellationToken);
        return _mapper.Map<VehiculoDto>(guardado);
    }
}
