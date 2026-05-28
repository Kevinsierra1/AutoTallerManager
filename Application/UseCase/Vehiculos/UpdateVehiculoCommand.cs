using MediatR;
using AutoMapper;
using Application.Abstractions;
using Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCase.Vehiculos;

public record UpdateVehiculoCommand(Guid Id, UpdateVehiculoDto Dto) : IRequest<VehiculoDto>;

public class UpdateVehiculoCommandHandler : IRequestHandler<UpdateVehiculoCommand, VehiculoDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    public UpdateVehiculoCommandHandler(IApplicationDbContext context, IMapper mapper) { _context = context; _mapper = mapper; }

    public async Task<VehiculoDto> Handle(UpdateVehiculoCommand request, CancellationToken cancellationToken)
    {
        var v = await _context.Vehiculos
            .Include(v => v.ModeloVehiculo).ThenInclude(m => m.Marca)
            .Include(v => v.Color)
            .FirstOrDefaultAsync(v => v.Id == request.Id && !v.Eliminado, cancellationToken)
            ?? throw new NotFoundException("Vehiculo", request.Id);

        var dto = request.Dto;
        if (dto.Placa != null) v.Placa = dto.Placa;
        if (dto.ColorId.HasValue) v.ColorId = dto.ColorId.Value;
        if (dto.KilometrajeActual.HasValue) v.KilometrajeActual = dto.KilometrajeActual.Value;
        if (dto.Observaciones != null) v.Observaciones = dto.Observaciones;
        if (dto.Activo.HasValue) v.Activo = dto.Activo.Value;
        v.ActualizadoEn = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return _mapper.Map<VehiculoDto>(v);
    }
}
