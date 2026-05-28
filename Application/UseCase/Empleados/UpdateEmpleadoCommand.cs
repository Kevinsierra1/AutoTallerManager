using MediatR;
using Application.Abstractions;
using Application.Common.Exceptions;
using AutoMapper;

namespace Application.UseCase.Empleados;

public record UpdateEmpleadoCommand(Guid Id, UpdateEmpleadoDto Dto) : IRequest<EmpleadoDto>;

public class UpdateEmpleadoCommandHandler : IRequestHandler<UpdateEmpleadoCommand, EmpleadoDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    public UpdateEmpleadoCommandHandler(IApplicationDbContext context, IMapper mapper) { _context = context; _mapper = mapper; }

    public async Task<EmpleadoDto> Handle(UpdateEmpleadoCommand request, CancellationToken cancellationToken)
    {
        var empleado = await _context.Empleados.FindAsync([request.Id], cancellationToken)
            ?? throw new NotFoundException("Empleado", request.Id);
        if (empleado.Eliminado) throw new NotFoundException("Empleado", request.Id);

        var dto = request.Dto;
        if (dto.Nombres != null) empleado.Nombres = dto.Nombres;
        if (dto.Apellidos != null) empleado.Apellidos = dto.Apellidos;
        if (dto.Telefono != null) empleado.Telefono = dto.Telefono;
        if (dto.Email != null) empleado.Email = dto.Email;
        if (dto.Especialidad != null) empleado.Especialidad = dto.Especialidad;
        if (dto.Activo.HasValue) empleado.Activo = dto.Activo.Value;
        empleado.ActualizadoEn = DateTime.UtcNow;

        await _context.SaveChangesAsync(cancellationToken);
        return _mapper.Map<EmpleadoDto>(empleado);
    }
}
